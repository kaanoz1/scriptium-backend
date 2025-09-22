using System.Xml.Linq;

namespace ScriptiumBackend.Models;

public class SitemapUrl
{
    /// <summary>
    /// Sıralama için küme numarası (statik = 0, scripture = 1, section = 2, ...).
    /// </summary>
    public int OrderKey { get; set; }

    /// <summary>
    /// Veri tabanındaki özgün Id (stabil sıralama için).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tam URL (ör. https://scriptium.dev/about).
    /// </summary>
    public required string Loc { get; set; }

    /// <summary>
    /// İçeriğin son güncellenme zamanı. null olabilir.
    /// </summary>
    public DateTime LastMod { get; init; } = DateTime.UtcNow;
}
public static class SitemapGenerator
{
    private const string SitemapNs = "http://www.sitemaps.org/schemas/sitemap/0.9";

    /// <summary>
    /// Verilen URL listesine göre sitemap XML string üretir.
    /// </summary>
    public static string GenerateSitemapXml(IEnumerable<SitemapUrl> urls)
    {
        var urlset = new XElement(XName.Get("urlset", SitemapNs));

        foreach (var url in urls)
        {
            var urlElement = new XElement(XName.Get("url", SitemapNs),
                new XElement(XName.Get("loc", SitemapNs), url.Loc)
            );


            urlElement.Add(new XElement(
                XName.Get("lastmod", SitemapNs),
                url.LastMod.ToString("yyyy-MM-dd")
            ));

            urlset.Add(urlElement);
        }

        var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), urlset);
        return doc.ToString(SaveOptions.DisableFormatting);
    }
}



    /// <summary>
    /// Google/Bing uyumlu <sitemapindex> XML üretir.
    /// RFC: https://www.sitemaps.org/protocol.html#index
    /// </summary>
    public static class SitemapIndexGenerator
    {
        private const string NS = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static XName N(string n) => XName.Get(n, NS);

        /// <summary>
        /// Basit kullanım: https://example.com + "/sitemap/{i}.xml" şablonuyla partCount kadar kayıt üretir.
        /// lastModProvider verilir ise her parça için lastmod eklenir (yyyy-MM-dd).
        /// </summary>
        /// <param name="baseUrl">Örn: https://scriptium.dev (sonunda / yok)</param>
        /// <param name="partCount">Toplam parça sayısı (>=1)</param>
        /// <param name="pathTemplate">Örn: "/sitemap/{i}.xml" veya "/sitemaps/part-{i}.xml"</param>
        /// <param name="lastModProvider">Parça indexine göre lastmod (UTC önerilir)</param>
        public static string GenerateIndexXml(
            string baseUrl,
            int partCount,
            string pathTemplate = "/sitemap/{i}.xml",
            Func<int, DateTime?>? lastModProvider = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("baseUrl is required.", nameof(baseUrl));
            if (partCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(partCount), "partCount must be >= 1.");

            baseUrl = TrimEndSlash(baseUrl);

            var index = new XElement(N("sitemapindex"));

            for (int i = 0; i < partCount; i++)
            {
                var relative = pathTemplate.Replace("{i}", i.ToString());
                var loc = Combine(baseUrl, relative);

                var sm = new XElement(N("sitemap"),
                    new XElement(N("loc"), loc)
                );

                var lm = lastModProvider?.Invoke(i);
                if (lm.HasValue)
                {
                    sm.Add(new XElement(N("lastmod"), lm.Value.ToString("yyyy-MM-dd")));
                }

                index.Add(sm);
            }

            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), index);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Gelişmiş kullanım: doğrudan (loc, lastmod) çiftleri ver.
        /// </summary>
        public static string GenerateIndexXml(IEnumerable<(string loc, DateTime? lastmod)> items)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            var index = new XElement(N("sitemapindex"));
            foreach (var (loc, lastmod) in items)
            {
                if (string.IsNullOrWhiteSpace(loc)) continue;

                var sm = new XElement(N("sitemap"),
                    new XElement(N("loc"), loc)
                );

                if (lastmod.HasValue)
                    sm.Add(new XElement(N("lastmod"), lastmod.Value.ToString("yyyy-MM-dd")));

                index.Add(sm);
            }

            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), index);
            return doc.ToString(SaveOptions.DisableFormatting);
        }

        // --- Helpers ---

        private static string Combine(string baseUrl, string relativePath)
        {
            relativePath ??= string.Empty;
            if (!relativePath.StartsWith("/"))
                relativePath = "/" + relativePath;

            return TrimEndSlash(baseUrl) + relativePath;
        }

        private static string TrimEndSlash(string url)
        {
            return string.IsNullOrEmpty(url) ? url : url.TrimEnd('/');
        }
    }
