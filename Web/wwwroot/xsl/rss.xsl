<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
<html>
<head>
    <title>Copilot That Jawn - RSS Feed</title>
    <style>
        body {
            font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen-Sans, Ubuntu, Cantarell, "Helvetica Neue", sans-serif;
            line-height: 1.6;
            max-width: 900px;
            margin: 0 auto;
            padding: 2rem;
            background-color: #f8f9fa;
            color: #212529;
        }
        .header {
            text-align: center;
            margin-bottom: 2rem;
            padding: 1rem;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .header img {
            max-width: 100px;
            height: auto;
            margin-bottom: 1rem;
        }
        .feed-info {
            margin-bottom: 2rem;
            padding: 1rem;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .item {
            margin-bottom: 1.5rem;
            padding: 1.5rem;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .item:hover {
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }
        .item h2 {
            margin-top: 0;
            color: #0066cc;
        }
        .item a {
            color: #0066cc;
            text-decoration: none;
        }
        .item a:hover {
            text-decoration: underline;
        }
        .meta {
            font-size: 0.9rem;
            color: #6c757d;
            margin-top: 1rem;
        }
        .tags {
            margin-top: 0.5rem;
        }
        .tag {
            display: inline-block;
            padding: 0.25rem 0.5rem;
            margin: 0.25rem;
            background-color: #e9ecef;
            border-radius: 4px;
            font-size: 0.85rem;
        }
        .subscribe-button {
            display: inline-block;
            padding: 0.5rem 1rem;
            background-color: #0066cc;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 1rem;
        }
        .subscribe-button:hover {
            background-color: #0056b3;
        }
    </style>
</head>
<body>
    <div class="header">
        <img src="/img/icon-with-bg.webp" alt="Copilot That Jawn"/>
        <h1><xsl:value-of select="rss/channel/title"/></h1>
        <p><xsl:value-of select="rss/channel/description"/></p>
        <a class="subscribe-button" href="feed.rss">Subscribe to RSS Feed</a>
    </div>

    <div class="feed-info">
        <p><strong>Last Updated:</strong> <xsl:value-of select="rss/channel/lastBuildDate"/></p>
        <p><strong>Copyright:</strong> <xsl:value-of select="rss/channel/copyright"/></p>
    </div>

    <div class="items">
        <xsl:for-each select="rss/channel/item">
            <div class="item">
                <h2><a href="{link}"><xsl:value-of select="title"/></a></h2>
                <p><xsl:value-of select="description"/></p>
                <div class="meta">
                    <div>Published: <xsl:value-of select="pubDate"/></div>
                    <div class="tags">
                        <xsl:for-each select="category">
                            <span class="tag"><xsl:value-of select="."/></span>
                        </xsl:for-each>
                    </div>
                </div>
            </div>
        </xsl:for-each>
    </div>
</body>
</html>
</xsl:template>
</xsl:stylesheet>
