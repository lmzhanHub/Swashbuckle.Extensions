using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Swashbuckle.SwaggerUi;

namespace Swashbuckle.Application
{
    public class SwaggerUiConfig
    {
        private readonly Dictionary<string, EmbeddedAssetDescriptor> _pathToAssetMap;
        private readonly Dictionary<string, string> _templateParams;
        private readonly Func<HttpRequestMessage, string> _rootUrlResolver;

        public SwaggerUiConfig(IEnumerable<string> discoveryPaths, Func<HttpRequestMessage, string> rootUrlResolver)
        {
            _pathToAssetMap = new Dictionary<string, EmbeddedAssetDescriptor>();

            _templateParams = new Dictionary<string, string>
            {
                { "%(DocumentTitle)", "Swagger UI" },
                { "%(StylesheetIncludes)", "" },
                { "%(DiscoveryPaths)", String.Join("|", discoveryPaths) },
                { "%(BooleanValues)", "true|false" },
                { "%(ValidatorUrl)", "" },
                { "%(CustomScripts)", "" },
                { "%(DocExpansion)", "none" },
                { "%(SupportedSubmitMethods)", "get|put|post|delete|options|head|patch" },
                { "%(OAuth2Enabled)", "false" },
                { "%(OAuth2ClientId)", "" },
                { "%(OAuth2ClientSecret)", "" },
                { "%(OAuth2Realm)", "" },
                { "%(OAuth2AppName)", "" },
                { "%(OAuth2ScopeSeperator)", " " },
                { "%(OAuth2AdditionalQueryStringParams)", "{}" },
                { "%(ApiKeyName)", "api_key" },
                { "%(ApiKeyIn)", "query" }
            };
            _rootUrlResolver = rootUrlResolver;

            MapPathsForSwaggerUiAssets();

            // Use some custom versions to support config and extensionless paths
            var thisAssembly = GetType().Assembly;
            CustomAsset("index", thisAssembly, "Swashbuckle.SwaggerUi.CustomAssets.index.html", isTemplate: true);
            CustomAsset("css/screen-css", thisAssembly, "Swashbuckle.SwaggerUi.CustomAssets.screen.css");
            CustomAsset("css/typography-css", thisAssembly, "Swashbuckle.SwaggerUi.CustomAssets.typography.css");
            string[] resourceSrcs = new string[26]
               {
                    "lib/object-assign-pollyfill-js",
                    "lib/jquery-1-8-0-min-js",
                    "lib/jquery-slideto-min-js",
                    "lib/jquery-wiggle-min-js",
                    "lib/jquery-ba-bbq-min-js",
                    "lib/handlebars-4-0-5-js",
                    "lib/lodash-min-js",
                    "lib/backbone-min-js",
                    "swagger-ui-min-js",
                    "lib/highlight-9-1-0-pack-js",
                    "lib/highlight-9-1-0-pack_extended-js",
                    "lib/jsoneditor-min-js",
                    "lib/marked-js",
                    "lib/swagger-oauth-js",
                    "lib/lang-zh-cn-js",
                    "lang/translator-js",
                    "css/print-css",
                    "css/reset-css",
                    "css/style-css",
                    "images/explorer_icons.png",
                    "images/favicon-16x16-png",
                    "images/favicon-32x32-png",
                    "images/logo_small.png",
                    "images/pet_store_api.png",
                    "images/wordnik_api.png",
                    "images/logo_small-png"
               };

            string[] resourceNames = new string[26]
            {
                "Swashbuckle.SwaggerUi.CustomAssets.lib-object-assign-pollyfill.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-jquery-1.8.0.min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-jquery.slideto.min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-jquery.wiggle.min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-jquery.ba-bbq.min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-handlebars-4.0.5.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-lodash.min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-backbone-min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.swagger-ui.min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-highlight.9.1.0.pack.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-highlight.9.1.0.pack_extended.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-jsoneditor.min.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-marked.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lib-swagger-oauth.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lang-zh-cn.js",
                "Swashbuckle.SwaggerUi.CustomAssets.lang-translator.js",
                "Swashbuckle.SwaggerUi.CustomAssets.css-print.css",
                "Swashbuckle.SwaggerUi.CustomAssets.css-reset.css",
                "Swashbuckle.SwaggerUi.CustomAssets.css-style.css",
                "Swashbuckle.SwaggerUi.CustomAssets.images-explorer_icons.png",
                "Swashbuckle.SwaggerUi.CustomAssets.images-favicon-16x16.png",
                "Swashbuckle.SwaggerUi.CustomAssets.images-favicon-32x32.png",
                "Swashbuckle.SwaggerUi.CustomAssets.images-logo_small.png",
                "Swashbuckle.SwaggerUi.CustomAssets.images-pet_store_api.png",
                "Swashbuckle.SwaggerUi.CustomAssets.images-wordnik_api.png",
                "Swashbuckle.SwaggerUi.CustomAssets.images-logo_small.png"
            };

            for (int i = 0; i < resourceSrcs.Length; i++)
            {
                CustomAsset(resourceSrcs[i], thisAssembly, resourceNames[i]);
            }
        }

        public void InjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen", bool isTemplate = false)
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(StylesheetIncludes)"]);
            stringBuilder.AppendLine("<link href='" + path + "' media='" + media + "' rel='stylesheet' type='text/css' />");
            _templateParams["%(StylesheetIncludes)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName, isTemplate);
        }

        public void BooleanValues(IEnumerable<string> values)
        {
            _templateParams["%(BooleanValues)"] = String.Join("|", values);
        }

        public void DocumentTitle(string title)
        {
            _templateParams["%(DocumentTitle)"] = title;
        }

        public void SetValidatorUrl(string url)
        {
            _templateParams["%(ValidatorUrl)"] = url;
        }

        public void DisableValidator()
        {
            _templateParams["%(ValidatorUrl)"] = "null";
        }

        public void InjectJavaScript(Assembly resourceAssembly, string resourceName, bool isTemplate = false)
        {
            var path = "ext/" + resourceName.Replace(".", "-");

            var stringBuilder = new StringBuilder(_templateParams["%(CustomScripts)"]);
            if (stringBuilder.Length > 0)
                stringBuilder.Append("|");

            stringBuilder.Append(path);
            _templateParams["%(CustomScripts)"] = stringBuilder.ToString();

            CustomAsset(path, resourceAssembly, resourceName, isTemplate);
        }

        public void DocExpansion(DocExpansion docExpansion)
        {
            _templateParams["%(DocExpansion)"] = docExpansion.ToString().ToLower();
        }

        public void SupportedSubmitMethods(params string[] methods)
        {
            _templateParams["%(SupportedSubmitMethods)"] = String.Join("|", methods).ToLower();
        }

        public void CustomAsset(string path, Assembly resourceAssembly, string resourceName, bool isTemplate = false)
        {
            if (path == "index") isTemplate = true;
            _pathToAssetMap[path] = new EmbeddedAssetDescriptor(resourceAssembly, resourceName, isTemplate);
        }

        public void EnableDiscoveryUrlSelector()
        {
            InjectJavaScript(GetType().Assembly, "Swashbuckle.SwaggerUi.CustomAssets.discoveryUrlSelector.js");
        }

        public void EnableOAuth2Support(string clientId, string realm, string appName)
        {
            EnableOAuth2Support(clientId, "N/A", realm, appName);
        }

        public void EnableOAuth2Support(
            string clientId,
            string clientSecret,
            string realm,
            string appName,
            string scopeSeperator = " ",
            Dictionary<string, string> additionalQueryStringParams = null)
        {
            _templateParams["%(OAuth2Enabled)"] = "true";
            _templateParams["%(OAuth2ClientId)"] = clientId;
            _templateParams["%(OAuth2ClientSecret)"] = clientSecret;
            _templateParams["%(OAuth2Realm)"] = realm;
            _templateParams["%(OAuth2AppName)"] = appName;
            _templateParams["%(OAuth2ScopeSeperator)"] = scopeSeperator;

            if (additionalQueryStringParams != null)
                _templateParams["%(OAuth2AdditionalQueryStringParams)"] = JsonConvert.SerializeObject(additionalQueryStringParams);
        }

        public void EnableApiKeySupport(string name, string apiKeyIn)
        {
            _templateParams["%(ApiKeyName)"] = name;
            _templateParams["%(ApiKeyIn)"] = apiKeyIn;
        }

        internal IAssetProvider GetSwaggerUiProvider()
        {
            return new EmbeddedAssetProvider(_pathToAssetMap, _templateParams);
        }

        internal string GetRootUrl(HttpRequestMessage swaggerRequest)
        {
            return _rootUrlResolver(swaggerRequest);
        }

        private void MapPathsForSwaggerUiAssets()
        {
            var thisAssembly = GetType().Assembly;
            foreach (var resourceName in thisAssembly.GetManifestResourceNames())
            {
                if (resourceName.Contains("Swashbuckle.SwaggerUi.CustomAssets")) continue; // original assets only

                var path = resourceName
                    .Replace("\\", "/")
                    .Replace(".", "-"); // extensionless to avoid RUMMFAR

                _pathToAssetMap[path] = new EmbeddedAssetDescriptor(thisAssembly, resourceName, path == "index");
            }
        }
    }

    public enum DocExpansion
    {
        None,
        List,
        Full
    }
}