// BlazingSpire overlay interop module
// Provides focus-trap, scroll-lock, and click-outside detection for overlay components.

/**
 * Creates a focus trap within a container element.
 * Traps Tab/Shift+Tab cycling and handles Escape key.
 * @param {HTMLElement} container
 * @param {object} dotNetRef - .NET object reference with HandleEscapeKey method
 * @returns {{ dispose: () => void }}
 */
export function createFocusTrap(container, dotNetRef) {
    const focusableSelector =
        'a[href], button:not([disabled]), textarea:not([disabled]), input:not([disabled]), ' +
        'select:not([disabled]), [tabindex]:not([tabindex="-1"])';

    const previouslyFocused = document.activeElement;

    function getFocusableElements() {
        return [...container.querySelectorAll(focusableSelector)].filter(
            el => !el.hasAttribute('disabled') && el.offsetParent !== null
        );
    }

    function onKeyDown(e) {
        if (e.key === 'Escape') {
            dotNetRef.invokeMethodAsync('HandleEscapeKey');
            return;
        }

        if (e.key !== 'Tab') return;

        const focusable = getFocusableElements();
        if (focusable.length === 0) {
            e.preventDefault();
            return;
        }

        const first = focusable[0];
        const last = focusable[focusable.length - 1];

        if (e.shiftKey) {
            if (document.activeElement === first) {
                e.preventDefault();
                last.focus();
            }
        } else {
            if (document.activeElement === last) {
                e.preventDefault();
                first.focus();
            }
        }
    }

    document.addEventListener('keydown', onKeyDown);

    // Focus the first focusable element
    requestAnimationFrame(() => {
        const focusable = getFocusableElements();
        if (focusable.length > 0) focusable[0].focus();
    });

    return {
        dispose() {
            document.removeEventListener('keydown', onKeyDown);
            if (previouslyFocused && typeof previouslyFocused.focus === 'function' && document.contains(previouslyFocused)) {
                previouslyFocused.focus();
            }
        }
    };
}

/**
 * Locks body scrolling by setting overflow: hidden.
 * @returns {{ dispose: () => void }}
 */
export function lockBodyScroll() {
    const original = document.body.style.overflow;
    const scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;

    document.body.style.overflow = 'hidden';
    if (scrollbarWidth > 0) {
        document.body.style.paddingRight = `${scrollbarWidth}px`;
    }

    return {
        dispose() {
            document.body.style.overflow = original;
            document.body.style.paddingRight = '';
        }
    };
}

/**
 * Registers a document-level Escape key listener.
 * Used by overlays that don't trap focus but still need to close on Escape.
 * @param {object} dotNetRef - .NET object reference with HandleEscapeKey method
 * @returns {{ dispose: () => void }}
 */
export function createEscapeHandler(dotNetRef) {
    function onKeyDown(e) {
        if (e.key === 'Escape') {
            dotNetRef.invokeMethodAsync('HandleEscapeKey');
        }
    }
    document.addEventListener('keydown', onKeyDown);
    return {
        dispose() {
            document.removeEventListener('keydown', onKeyDown);
        }
    };
}

/**
 * Detects clicks outside a container element.
 * @param {HTMLElement} container
 * @param {object} dotNetRef - .NET object reference with HandleInteractOutside method
 * @returns {{ dispose: () => void }}
 */
export function onClickOutside(container, dotNetRef) {
    function onPointerDown(e) {
        if (container && !container.contains(e.target)) {
            dotNetRef.invokeMethodAsync('HandleInteractOutside');
        }
    }

    // Delay attachment to avoid catching the opening click
    requestAnimationFrame(() => {
        document.addEventListener('pointerdown', onPointerDown);
    });

    return {
        dispose() {
            document.removeEventListener('pointerdown', onPointerDown);
        }
    };
}
