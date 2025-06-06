// Analytics event tracking
const Analytics = {
    // Track page views (called automatically by GA4)
    pageView: (title, path) => {
        gtag('event', 'page_view', {
            page_title: title,
            page_path: path
        });
    },

    // Track tip views
    trackTipView: (tipTitle, category) => {
        gtag('event', 'tip_view', {
            tip_title: tipTitle,
            category: category
        });
    },

    // Track category views
    trackCategoryView: (category) => {
        gtag('event', 'category_view', {
            category: category
        });
    },

    // Track tag clicks
    trackTagClick: (tag) => {
        gtag('event', 'tag_click', {
            tag: tag
        });
    },

    // Track difficulty filter usage
    trackDifficultyFilter: (difficulty) => {
        gtag('event', 'difficulty_filter', {
            difficulty: difficulty
        });
    },

    // Track search queries
    trackSearch: (query) => {
        gtag('event', 'search', {
            search_term: query
        });
    }
};

// Export for use in other files
window.Analytics = Analytics;
