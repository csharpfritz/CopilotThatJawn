// Development Analytics Stub
// This file provides a stub for Analytics functions in development
// to prevent errors when analytics.js is not loaded

if (typeof Analytics === 'undefined') {
    window.Analytics = {
        isEnabled: () => false,
        pageView: () => {},
        trackTipView: () => {},
        trackCategoryView: () => {},
        trackTagClick: () => {},
        trackDifficultyFilter: () => {},
        trackSearch: () => {},
        trackShare: () => {}
    };
}
