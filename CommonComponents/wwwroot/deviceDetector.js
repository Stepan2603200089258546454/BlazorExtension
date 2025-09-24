export function getDeviceInfo() {
    const userAgent = navigator.userAgent.toLowerCase();
    const isMobile = /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(userAgent);
    const isTablet = /ipad|android(?=.*mobile)/i.test(userAgent);

    return {
        isMobile: isMobile,
        isTablet: isTablet,
        isDesktop: !isMobile && !isTablet,
        userAgent: navigator.userAgent,
        screenWidth: window.screen.width,
        screenHeight: window.screen.height
    };
}
export function getViewportSize() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}