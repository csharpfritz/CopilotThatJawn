// Analytics event tracking
const Analytics = {
    // Check if analytics is available (only in production)
    isEnabled: () => {
        return typeof gtag !== 'undefined';
    },

    // Track page views (called automatically by GA4)
    pageView: (title, path) => {
        if (Analytics.isEnabled()) {
            gtag('event', 'page_view', {
                page_title: title,
                page_path: path
            });
        }
    },

    // Track tip views
    trackTipView: (tipTitle, category) => {
        if (Analytics.isEnabled()) {
            gtag('event', 'tip_view', {
                tip_title: tipTitle,
                category: category
            });
        }
    },

    // Track category views
    trackCategoryView: (category) => {
        if (Analytics.isEnabled()) {
            gtag('event', 'category_view', {
                category: category
            });
        }
    },

    // Track tag clicks
    trackTagClick: (tag) => {
        if (Analytics.isEnabled()) {
            gtag('event', 'tag_click', {
                tag: tag
            });
        }
    },

    // Track difficulty filter usage
    trackDifficultyFilter: (difficulty) => {
        if (Analytics.isEnabled()) {
            gtag('event', 'difficulty_filter', {
                difficulty: difficulty
            });
        }
    },    // Track search queries
    trackSearch: (query) => {
        if (Analytics.isEnabled()) {
            gtag('event', 'search', {
                search_term: query
            });
        }
    },

    // Track social shares
    trackShare: (method, contentType, itemId) => {
        if (Analytics.isEnabled()) {
            gtag('event', 'share', {
                method: method,
                content_type: contentType,
                item_id: itemId
            });
        }
    }
};

// Export for use in other files
window.Analytics = Analytics;
