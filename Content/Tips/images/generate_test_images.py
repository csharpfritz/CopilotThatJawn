import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns
from pathlib import Path

# Create images directory if it doesn't exist
images_dir = Path(__file__).parent
images_dir.mkdir(exist_ok=True)

# Set style
plt.style.use('seaborn-v0_8')
sns.set_theme()
sns.set_palette("husl")

# 1. Create a sample line plot showing "AI Productivity over time"
def create_productivity_chart():
    plt.figure(figsize=(10, 6))
    x = np.array([2020, 2021, 2022, 2023, 2024, 2025])
    y = np.array([10, 15, 25, 45, 75, 100])
    
    plt.plot(x, y, marker='o', linewidth=2, markersize=8)
    plt.title("Developer Productivity with AI Tools", fontsize=14)
    plt.xlabel("Year")
    plt.ylabel("Productivity Score")
    plt.grid(True, alpha=0.3)
    
    plt.savefig(images_dir / 'productivity-trend.png', dpi=300, bbox_inches='tight')
    plt.close()

# 2. Create a bar chart showing "Popular AI Tools Usage"
def create_tools_usage_chart():
    plt.figure(figsize=(10, 6))
    tools = ['GitHub Copilot', 'Microsoft 365\nCopilot', 'Copilot\nStudio', 'Azure\nCopilot']
    usage = [85, 65, 45, 40]
    
    plt.bar(tools, usage, color=sns.color_palette("husl", 4))
    plt.title("AI Tools Adoption Rate (2025)", fontsize=14)
    plt.ylabel("Usage Percentage")
    plt.ylim(0, 100)
    
    # Add percentage labels on top of each bar
    for i, v in enumerate(usage):
        plt.text(i, v + 1, f'{v}%', ha='center', fontsize=10)
    
    plt.savefig(images_dir / 'tools-usage.png', dpi=300, bbox_inches='tight')
    plt.close()

# 3. Create a pie chart showing "Time saved by category"
def create_time_savings_chart():
    plt.figure(figsize=(10, 8))
    categories = ['Code Generation', 'Documentation', 'Testing', 'Code Review', 'Other']
    sizes = [35, 25, 20, 15, 5]
    explode = (0.1, 0, 0, 0, 0)
    
    plt.pie(sizes, explode=explode, labels=categories, autopct='%1.0f%%',
            shadow=True, startangle=90)
    plt.title("Time Saved by AI Tools - By Category", fontsize=14)
    
    plt.savefig(images_dir / 'time-savings.png', dpi=300, bbox_inches='tight')
    plt.close()

if __name__ == "__main__":
    print("Generating test images...")
    create_productivity_chart()
    create_tools_usage_chart()
    create_time_savings_chart()
    print("Done! Images have been saved to the images directory.")
