// Font Switcher Utility for Code Blocks
// Author: Copilot That Jawn Team

class CodeFontSwitcher {
    constructor() {
        this.fonts = {
            'jetbrains': 'JetBrains Mono',
            'fira': 'Fira Code', 
            'source': 'Source Code Pro',
            'cascadia': 'Cascadia Code',
            'sf': 'SF Mono'
        };
        
        this.currentFont = this.loadSavedFont() || 'jetbrains';
        this.init();
    }
    
    init() {
        this.applyFont(this.currentFont);
        this.createFontSelector();
    }    createFontSelector() {
        // Only create if there are code blocks on the page
        const codeBlocks = document.querySelectorAll('pre[class*="language-"]');
        if (codeBlocks.length === 0) return;
        
        const selector = document.createElement('div');
        selector.className = 'code-font-selector position-fixed';
        selector.style.cssText = `
            top: 20px;
            right: 20px;
            z-index: 1050;
            background: white;
            border: 1px solid #dee2e6;
            border-radius: 0.375rem;
            padding: 0.5rem;
            box-shadow: 0 0.125rem 0.25rem rgba(0,0,0,0.075);
            font-size: 0.875rem;
        `;
        
        const label = document.createElement('label');
        label.textContent = 'Code Font: ';
        label.style.marginRight = '0.5rem';
        
        const select = document.createElement('select');
        select.className = 'form-select form-select-sm';
        select.style.width = 'auto';
        select.style.display = 'inline-block';
        
        Object.entries(this.fonts).forEach(([key, name]) => {
            const option = document.createElement('option');
            option.value = key;
            option.textContent = name;
            option.selected = key === this.currentFont;
            select.appendChild(option);
        });
        
        select.addEventListener('change', (e) => {
            this.switchFont(e.target.value);
        });
        
        selector.appendChild(label);
        selector.appendChild(select);
        document.body.appendChild(selector);
    }
    
    switchFont(fontKey) {
        this.currentFont = fontKey;
        this.applyFont(fontKey);
        this.saveFontPreference(fontKey);
    }
    
    applyFont(fontKey) {
        // Remove all font classes from body
        document.body.classList.remove(...Object.keys(this.fonts).map(f => `code-font-${f}`));
        
        // Apply new font class
        document.body.classList.add(`code-font-${fontKey}`);
    }
    
    saveFontPreference(fontKey) {
        try {
            localStorage.setItem('copilot-jawn-code-font', fontKey);
        } catch (e) {
            console.warn('Unable to save font preference:', e);
        }
    }
    
    loadSavedFont() {
        try {
            return localStorage.getItem('copilot-jawn-code-font');
        } catch (e) {
            console.warn('Unable to load font preference:', e);
            return null;
        }
    }
}

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    // Only initialize if there are code blocks on the page
    if (document.querySelectorAll('pre[class*="language-"]').length > 0) {
        // Don't initialize font switcher on details pages
        if (window.location.pathname.includes('/tips/') && !window.location.pathname.endsWith('/tips/')) {
            return;
        }
        new CodeFontSwitcher();
    }
});

// Copy to clipboard functionality for code blocks
document.addEventListener('DOMContentLoaded', () => {
    // Add copy buttons to code blocks
    const codeBlocks = document.querySelectorAll('pre[class*="language-"]');
    
    codeBlocks.forEach((block, index) => {
        // Create copy button
        const copyBtn = document.createElement('button');
        copyBtn.className = 'btn btn-sm btn-outline-secondary copy-code-btn';
        copyBtn.innerHTML = '<i class="bi bi-clipboard"></i>';
        copyBtn.title = 'Copy code to clipboard';
        copyBtn.style.cssText = `
            position: absolute;
            top: 0.5rem;
            right: 0.5rem;
            z-index: 10;
            opacity: 0;
            transition: opacity 0.2s;
        `;
        
        // Position the parent block relatively
        block.style.position = 'relative';
        
        // Show/hide button on hover
        block.addEventListener('mouseenter', () => {
            copyBtn.style.opacity = '1';
        });
        
        block.addEventListener('mouseleave', () => {
            copyBtn.style.opacity = '0';
        });
        
        // Copy functionality
        copyBtn.addEventListener('click', async () => {
            const code = block.querySelector('code');
            const text = code.textContent || code.innerText;
            
            try {
                await navigator.clipboard.writeText(text);
                
                // Show success feedback
                const originalContent = copyBtn.innerHTML;
                copyBtn.innerHTML = '<i class="bi bi-check"></i>';
                copyBtn.classList.remove('btn-outline-secondary');
                copyBtn.classList.add('btn-success');
                
                setTimeout(() => {
                    copyBtn.innerHTML = originalContent;
                    copyBtn.classList.remove('btn-success');
                    copyBtn.classList.add('btn-outline-secondary');
                }, 2000);
                
            } catch (err) {
                console.error('Failed to copy code:', err);
                
                // Fallback for older browsers
                const textArea = document.createElement('textarea');
                textArea.value = text;
                document.body.appendChild(textArea);
                textArea.select();
                
                try {
                    document.execCommand('copy');
                    copyBtn.innerHTML = '<i class="bi bi-check"></i>';
                    copyBtn.classList.add('btn-success');
                } catch (fallbackErr) {
                    console.error('Fallback copy failed:', fallbackErr);
                }
                
                document.body.removeChild(textArea);
            }
        });
        
        block.appendChild(copyBtn);
    });
});
