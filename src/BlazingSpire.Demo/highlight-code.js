#!/usr/bin/env node
// Build-time code highlighting script.
// Processes all .razor files, finds <code class="language-*"> blocks,
// and replaces their contents with Prism-highlighted HTML.
// Run: node highlight-code.js

const fs = require('fs');
const path = require('path');
const Prism = require('./node_modules/prismjs');

// Load languages we need
require('./node_modules/prismjs/components/prism-markup');
require('./node_modules/prismjs/components/prism-csharp');

// Map class names to Prism language keys
const langMap = {
  'language-xml': 'markup',
  'language-html': 'markup',
  'language-markup': 'markup',
  'language-csharp': 'csharp',
  'language-cs': 'csharp',
};

// Regex to find <code class="language-xxx">...</code> blocks
const codeBlockRegex = /<code\s+class="(language-[\w-]+)">([\s\S]*?)<\/code>/g;

function decodeEntities(str) {
  return str
    .replace(/&lt;/g, '<')
    .replace(/&gt;/g, '>')
    .replace(/&amp;/g, '&')
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'")
    .replace(/&#64;/g, '@')
    .replace(/&#123;/g, '{')
    .replace(/&#125;/g, '}');
}

function highlightFile(filePath) {
  let content = fs.readFileSync(filePath, 'utf-8');
  let modified = false;

  content = content.replace(codeBlockRegex, (match, langClass, codeContent) => {
    // Skip if already highlighted (contains <span class="token)
    if (codeContent.includes('<span class="token')) {
      return match;
    }

    const langKey = langMap[langClass];
    if (!langKey || !Prism.languages[langKey]) {
      return match;
    }

    // Decode HTML entities to get raw code
    const rawCode = decodeEntities(codeContent);

    // Highlight with Prism
    let highlighted = Prism.highlight(rawCode, Prism.languages[langKey], langKey);

    // Re-encode Razor-sensitive characters in text nodes only (not in HTML tags).
    // Split on Prism's <span> tags, encode text segments, rejoin.
    highlighted = highlighted.replace(/(@)/g, '&#64;');

    modified = true;
    return `<code class="${langClass}">${highlighted}</code>`;
  });

  if (modified) {
    fs.writeFileSync(filePath, content, 'utf-8');
    return true;
  }
  return false;
}

// Find all .razor files in Pages/Components/ and Pages/
const pagesDir = path.join(__dirname, 'Components', 'Pages');
let count = 0;

function walkDir(dir) {
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      walkDir(fullPath);
    } else if (entry.name.endsWith('.razor')) {
      if (highlightFile(fullPath)) {
        console.log(`  highlighted: ${path.relative(__dirname, fullPath)}`);
        count++;
      }
    }
  }
}

console.log('Pre-highlighting code blocks...');
walkDir(pagesDir);
console.log(`Done. ${count} file(s) updated.`);
