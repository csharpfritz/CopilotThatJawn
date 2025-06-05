/**
 * Theme Switcher for Copilot That Jawn
 * Provides manual light/dark theme switching functionality
 */
class ThemeSwitcher {
    constructor() {
        this.themes = {
            light: {
                name: 'Light',
                icon: 'bi-sun-fill',
                class: 'theme-light'
            },
            dark: {
                name: 'Dark',
                icon: 'bi-moon-stars-fill',
                class: 'theme-dark'
            },
            auto: {
                name: 'Auto',
                icon: 'bi-circle-half',
                class: 'theme-auto'
            }
        };
        
        this.currentTheme = this.getSavedTheme() || 'auto';
        this.mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    }
    
    init() {
        this.createThemeToggle();
        this.applyTheme(this.currentTheme);
        
        // Listen for system theme changes when in auto mode
        this.mediaQuery.addEventListener('change', () => {
            if (this.currentTheme === 'auto') {
                this.applyTheme('auto');
            }
        });
        
        // Initialize immediately
        document.addEventListener('DOMContentLoaded', () => {
            this.applyTheme(this.currentTheme);
        });
    }
      createThemeToggle() {
        // Find the theme toggle container in the navbar
        const container = document.getElementById('themeToggleContainer');
        if (!container) {
            console.warn('Theme toggle container not found');
            return;
        }
        
        // Create theme toggle dropdown
        container.className = 'nav-item dropdown';
          container.innerHTML = `
            <button class="btn btn-link nav-link dropdown-toggle" 
                    type="button" 
                    id="themeDropdown" 
                    data-bs-toggle="dropdown" 
                    aria-expanded="false"
                    title="Change theme">
                <i class="bi ${this.themes[this.currentTheme].icon}" id="themeIcon"></i>
                <span class="d-none d-md-inline ms-1" id="themeLabel">${this.themes[this.currentTheme].name}</span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="themeDropdown" style="z-index: 1055;">
                <li><button class="dropdown-item" type="button" data-theme="light">
                    <i class="bi bi-sun-fill me-2"></i>Light
                </button></li>
                <li><button class="dropdown-item" type="button" data-theme="dark">
                    <i class="bi bi-moon-stars-fill me-2"></i>Dark
                </button></li>
                <li><button class="dropdown-item" type="button" data-theme="auto">
                    <i class="bi bi-circle-half me-2"></i>Auto
                </button></li>
            </ul>
        `;
        
        // Add event listeners
        const dropdownItems = container.querySelectorAll('[data-theme]');
        dropdownItems.forEach(item => {
            item.addEventListener('click', (e) => {
                const theme = e.currentTarget.getAttribute('data-theme');
                this.switchTheme(theme);
            });
        });
    }
    
    switchTheme(theme) {
        this.currentTheme = theme;
        this.applyTheme(theme);
        this.saveTheme(theme);
        this.updateToggleUI();
    }
    
    applyTheme(theme) {
        const body = document.body;
        const html = document.documentElement;
        
        // Remove all theme classes
        Object.values(this.themes).forEach(themeObj => {
            body.classList.remove(themeObj.class);
            html.classList.remove(themeObj.class);
        });
        
        // Apply the new theme class
        const themeClass = this.themes[theme].class;
        body.classList.add(themeClass);
        html.classList.add(themeClass);
        
        // Set data attribute for CSS targeting
        html.setAttribute('data-theme', theme);
        
        // Handle auto theme
        if (theme === 'auto') {
            const prefersDark = this.mediaQuery.matches;
            html.setAttribute('data-theme-resolved', prefersDark ? 'dark' : 'light');
        } else {
            html.setAttribute('data-theme-resolved', theme);
        }
          // Update navbar styling based on theme
        this.updateNavbarStyling();
        
        // Dispatch custom event for other components to react to theme changes
        document.dispatchEvent(new CustomEvent('theme-changed', {
            detail: {
                theme: theme,
                resolvedTheme: html.getAttribute('data-theme-resolved')
            }
        }));
    }
      updateNavbarStyling() {
        const navbar = document.querySelector('.navbar');
        const navLinks = document.querySelectorAll('.nav-link');
        const resolvedTheme = document.documentElement.getAttribute('data-theme-resolved');
        
        if (navbar) {
            navbar.classList.remove('navbar-light', 'navbar-dark', 'bg-white', 'bg-dark');
            
            if (resolvedTheme === 'dark') {
                navbar.classList.add('navbar-dark', 'bg-dark');
                
                // Update all nav links to remove text-dark class for dark theme
                navLinks.forEach(link => {
                    link.classList.remove('text-dark');
                });
                
                // Update dropdown toggle button text color
                const themeDropdown = document.getElementById('themeDropdown');
                if (themeDropdown) {
                    themeDropdown.classList.remove('text-dark');
                }
            } else {
                navbar.classList.add('navbar-light', 'bg-white');
                
                // For light theme, we let Bootstrap's defaults handle the colors
            }
        }
    }
    
    updateToggleUI() {
        const themeIcon = document.getElementById('themeIcon');
        const themeLabel = document.getElementById('themeLabel');
        
        if (themeIcon && themeLabel) {
            const theme = this.themes[this.currentTheme];
            themeIcon.className = `bi ${theme.icon}`;
            themeLabel.textContent = theme.name;
        }
        
        // Update active state in dropdown
        const dropdownItems = document.querySelectorAll('[data-theme]');
        dropdownItems.forEach(item => {
            const theme = item.getAttribute('data-theme');
            item.classList.toggle('active', theme === this.currentTheme);
        });
    }
    
    saveTheme(theme) {
        try {
            localStorage.setItem('copilot-jawn-theme', theme);
        } catch (e) {
            console.warn('Unable to save theme preference:', e);
        }
    }
    
    getSavedTheme() {
        try {
            return localStorage.getItem('copilot-jawn-theme');
        } catch (e) {
            console.warn('Unable to load theme preference:', e);
            return null;
        }
    }
}

// Initialize theme switcher
const themeSwitcher = new ThemeSwitcher();

// Initialize when DOM is ready or immediately if already loaded
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => themeSwitcher.init());
} else {
    themeSwitcher.init();
}

// Export for potential external use
window.themeSwitcher = themeSwitcher;
