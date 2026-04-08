// BlazingSpire floating positioning module
// Lightweight Floating UI alternative — positions a floating element relative to a reference element.
// Handles viewport boundary detection and automatic side-flipping.

/**
 * Computes and applies position for a floating element relative to a reference element.
 * @param {HTMLElement} referenceEl - The anchor/trigger element
 * @param {HTMLElement} floatingEl - The floating/popup element
 * @param {object} options
 * @param {string} options.side - "top" | "bottom" | "left" | "right"
 * @param {string} options.align - "start" | "center" | "end"
 * @param {number} options.sideOffset - Gap between reference and floating element (px)
 * @param {number} options.alignOffset - Offset along the alignment axis (px)
 * @returns {{ update: () => void, dispose: () => void }}
 */
export function computePosition(referenceEl, floatingEl, options = {}) {
    const {
        side = 'bottom',
        align = 'center',
        sideOffset = 4,
        alignOffset = 0,
    } = options;

    function getPosition() {
        const refRect = referenceEl.getBoundingClientRect();
        const floatRect = floatingEl.getBoundingClientRect();
        const vw = window.innerWidth;
        const vh = window.innerHeight;

        let actualSide = side;

        // Flip if overflowing viewport
        if (side === 'bottom' && refRect.bottom + sideOffset + floatRect.height > vh && refRect.top - sideOffset - floatRect.height > 0) {
            actualSide = 'top';
        } else if (side === 'top' && refRect.top - sideOffset - floatRect.height < 0 && refRect.bottom + sideOffset + floatRect.height < vh) {
            actualSide = 'bottom';
        } else if (side === 'right' && refRect.right + sideOffset + floatRect.width > vw && refRect.left - sideOffset - floatRect.width > 0) {
            actualSide = 'left';
        } else if (side === 'left' && refRect.left - sideOffset - floatRect.width < 0 && refRect.right + sideOffset + floatRect.width < vw) {
            actualSide = 'right';
        }

        let top, left;

        // Calculate position based on side
        switch (actualSide) {
            case 'top':
                top = refRect.top - floatRect.height - sideOffset;
                left = getAlignedLeft(refRect, floatRect, align, alignOffset);
                break;
            case 'bottom':
                top = refRect.bottom + sideOffset;
                left = getAlignedLeft(refRect, floatRect, align, alignOffset);
                break;
            case 'left':
                left = refRect.left - floatRect.width - sideOffset;
                top = getAlignedTop(refRect, floatRect, align, alignOffset);
                break;
            case 'right':
                left = refRect.right + sideOffset;
                top = getAlignedTop(refRect, floatRect, align, alignOffset);
                break;
        }

        // Clamp to viewport
        left = Math.max(4, Math.min(left, vw - floatRect.width - 4));
        top = Math.max(4, Math.min(top, vh - floatRect.height - 4));

        return { top, left, side: actualSide };
    }

    function applyPosition() {
        const pos = getPosition();
        floatingEl.style.position = 'fixed';
        floatingEl.style.top = `${pos.top}px`;
        floatingEl.style.left = `${pos.left}px`;
        floatingEl.dataset.side = pos.side;
    }

    // Initial positioning
    applyPosition();

    // Reposition on scroll/resize
    const onUpdate = () => requestAnimationFrame(applyPosition);
    window.addEventListener('scroll', onUpdate, { passive: true, capture: true });
    window.addEventListener('resize', onUpdate, { passive: true });

    return {
        update: applyPosition,
        dispose() {
            window.removeEventListener('scroll', onUpdate, { capture: true });
            window.removeEventListener('resize', onUpdate);
        }
    };
}

function getAlignedLeft(refRect, floatRect, align, offset) {
    switch (align) {
        case 'start': return refRect.left + offset;
        case 'end': return refRect.right - floatRect.width + offset;
        case 'center':
        default: return refRect.left + (refRect.width - floatRect.width) / 2 + offset;
    }
}

function getAlignedTop(refRect, floatRect, align, offset) {
    switch (align) {
        case 'start': return refRect.top + offset;
        case 'end': return refRect.bottom - floatRect.height + offset;
        case 'center':
        default: return refRect.top + (refRect.height - floatRect.height) / 2 + offset;
    }
}
