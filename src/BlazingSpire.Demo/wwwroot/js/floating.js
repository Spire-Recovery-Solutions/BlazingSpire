// BlazingSpire floating positioning module
// Thin wrapper over @floating-ui/dom for Blazor JS interop.

import {
    computePosition as floatingCompute,
    autoUpdate,
    flip,
    shift,
    offset,
} from './floating-ui.dom.min.mjs';

/**
 * Positions a floating element relative to a reference element.
 * Uses Floating UI for battle-tested positioning with automatic
 * flipping, shifting, and viewport boundary detection.
 *
 * @param {HTMLElement} referenceEl - The anchor/trigger element
 * @param {HTMLElement} floatingEl - The floating/popup element
 * @param {object} options
 * @param {string} options.side - "top" | "bottom" | "left" | "right"
 * @param {string} options.align - "start" | "center" | "end"
 * @param {number} options.sideOffset - Gap between reference and floating element (px)
 * @param {number} options.alignOffset - Offset along the alignment axis (px)
 * @returns {{ dispose: () => void }}
 */
export function computePosition(referenceEl, floatingEl, options = {}) {
    const {
        side = 'bottom',
        align = 'start',
        sideOffset = 4,
        alignOffset = 0,
    } = options;

    // Map side/align to Floating UI placement string
    // Floating UI uses: "bottom", "bottom-start", "bottom-end", "top", "top-start", etc.
    const placement = align === 'center' ? side : `${side}-${align}`;

    // autoUpdate repositions on scroll, resize, and DOM mutations
    const cleanup = autoUpdate(referenceEl, floatingEl, () => {
        floatingCompute(referenceEl, floatingEl, {
            placement,
            middleware: [
                offset({ mainAxis: sideOffset, crossAxis: alignOffset }),
                flip(),
                shift({ padding: 4 }),
            ],
        }).then(({ x, y, placement: actualPlacement }) => {
            Object.assign(floatingEl.style, {
                position: 'fixed',
                left: `${x}px`,
                top: `${y}px`,
            });
            floatingEl.dataset.side = actualPlacement.split('-')[0];
        });
    });

    return { dispose: cleanup };
}
