export function getUserAgent() {
    return navigator.userAgent;
}
export function getViewportSize() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}