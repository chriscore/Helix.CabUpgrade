using Helix.CabUpgrade.Utils;
using Helix.CabUpgrade.Utils.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;
using System.Text;

namespace Helix.CabUpgrade.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPresetUpdater _presetUpdater;
        private readonly Settings _settings;

        public IndexModel(ILogger<IndexModel> logger, IPresetUpdater updater, IOptionsSnapshot<Settings> options)
        {
            _logger = logger;
            _presetUpdater = updater;
            _settings = options.Value;
        }

        public void OnGet()
        { }

        [BindProperty]
        public IFormFile Upload { get; set; }
        [BindProperty]
        public string PrimaryCabOverride { get; set; }
        [BindProperty]
        public string SecondaryCabOverride { get; set; }
        public async Task<FileContentResult> OnPostAsync()
        {
            var json = await ReadFormFileAsync(Upload);
            _logger.LogInformation(json);
            
            var result = _presetUpdater.UpdatePresetJson(json, new PresetUpdaterDefaults());

            byte[] bytes = Encoding.ASCII.GetBytes(result.PresetJson);
            return File(bytes, Upload.ContentType, $"{result.PatchName}.hlx");
        }

        public static Task<string?> ReadFormFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Task.FromResult((string)null);
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                return reader.ReadToEndAsync();
            }
        }
    }
}