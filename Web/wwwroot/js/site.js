// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Enhanced code block functionality
function initializeCopyButtons() {
    // Add language labels and copy buttons to code blocks
    const codeBlocks = document.querySelectorAll('pre[class*="language-"]');
    
    codeBlocks.forEach(function(block) {
        // Skip if header already exists (prevents duplicates)
        if (block.querySelector('.code-header')) {
            return;
        }
        
        // Extract language from class name
        const languageClass = Array.from(block.classList).find(cls => cls.startsWith('language-'));
        if (languageClass) {
            const language = languageClass.replace('language-', '');
            
            // Set data-language attribute for CSS styling
            block.setAttribute('data-language', language);
            
            // Ensure the language class is on both pre and code elements
            const codeElement = block.querySelector('code');
            if (codeElement && !codeElement.classList.contains(languageClass)) {
                codeElement.classList.add(languageClass);
            }
            
            // Create a header container for language label and copy button
            const headerContainer = document.createElement('div');
            headerContainer.className = 'code-header';
            
            // Create language label
            const languageLabel = document.createElement('span');
            languageLabel.className = 'language-label';
            languageLabel.textContent = language.toUpperCase();
            
            // Create copy button
            const copyButton = document.createElement('button');
            copyButton.className = 'copy-button';
            copyButton.textContent = 'Copy';
            copyButton.addEventListener('click', function() {
                const code = block.querySelector('code');
                if (code) {
                    navigator.clipboard.writeText(code.textContent).then(function() {
                        copyButton.textContent = 'Copied!';
                        setTimeout(function() {
                            copyButton.textContent = 'Copy';
                        }, 2000);
                    }).catch(function() {
                        // Fallback for browsers that don't support clipboard API
                        const textArea = document.createElement('textarea');
                        textArea.value = code.textContent;
                        document.body.appendChild(textArea);
                        textArea.select();
                        document.execCommand('copy');
                        document.body.removeChild(textArea);
                        
                        copyButton.textContent = 'Copied!';
                        setTimeout(function() {
                            copyButton.textContent = 'Copy';
                        }, 2000);
                    });
                }
            });
            
            // Add elements to header container
            headerContainer.appendChild(languageLabel);
            headerContainer.appendChild(copyButton);
            
            // Insert header at the beginning of the code block
            block.insertBefore(headerContainer, block.firstChild);
        }
    });
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize syntax highlighting first
    if (window.Prism) {
        Prism.highlightAll();
    }
    
    // Then initialize copy buttons for code blocks
    setTimeout(() => {
        initializeCopyButtons();
    }, 100); // Small delay to ensure Prism is done
});

// Also initialize when the page is fully loaded (for async content)
window.addEventListener('load', function() {
    if (window.Prism) {
        Prism.highlightAll();
    }
    
    // Re-initialize copy buttons in case new content was loaded
    setTimeout(() => {
        initializeCopyButtons();
    }, 100);
});

// Re-export for manual testing if needed
window.initializeCopyButtons = initializeCopyButtons;
