﻿using Helix.CabUpgrade.Utils;
using Helix.CabUpgrade.Utils.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text;

namespace Helix.CabUpgrade.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPresetUpdater _presetUpdater;
        public readonly Settings _settings;

        public static NewCabRepository NewCabRepository { get; set; }
        

        public IndexModel(ILogger<IndexModel> logger, IPresetUpdater updater, IOptionsSnapshot<Settings> options)
        {
            _logger = logger;
            _presetUpdater = updater;
            _settings = options.Value;
            NewCabRepository = new NewCabRepository(_settings);
        }

        public void OnGet()
        { }

        [BindProperty]
        public IFormFile Upload { get; set; }
        [BindProperty]
        public string SelectedPrimaryCab { get; set; }
        [BindProperty]
        public string SelectedSecondaryCab { get; set; }
        [BindProperty]
        public bool ForceOverridePrimaryCab { get; set; }
        [BindProperty]
        public bool ForceOverrideSecondaryCab { get; set; }

        [BindProperty]
        public bool IsError { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }

        public async Task<ActionResult> OnPostAsync()
        {
            try
            {
                if (Upload == null || Upload.Length == 0)
                {
                    throw new InvalidOperationException("Please upload a valid helix .hlx preset file, using the 'Choose file' button.");
                }
                var json = await ReadFormFileAsync(Upload);
                if (string.IsNullOrEmpty(json))
                {
                    throw new InvalidOperationException("Please upload a valid helix .hlx preset file, using the 'Choose file' button.");
                }
                _logger.LogInformation($"Received preset upload: {json}");

                var defaults = new PresetUpdaterDefaults()
                {
                    CabModelPrimaryOverride = SelectedPrimaryCab,
                    CabModelSecondaryOrAmpCabOverride = SelectedSecondaryCab,
                    ForceOverridePrimaryCab = ForceOverridePrimaryCab,
                    ForceOverrideSecondaryCab = ForceOverrideSecondaryCab
                };
                var result = _presetUpdater.UpdatePresetJson(json, defaults);

                byte[] bytes = Encoding.ASCII.GetBytes(result.PresetJson);
                ErrorMessage = "";
                IsError = false;
                return File(bytes, Upload.ContentType, $"{result.PatchName}.hlx");
            }
            catch (Exception e)
            {
                /*
                _logger.LogError(e, "Error occurred during user preset upgrade. Please get in touch via email to report a bug to chriscore361@googlemail.com");
                Response.StatusCode = 400;
                return Content($"{e.Message}{Environment.NewLine}{Environment.NewLine}{e.StackTrace}");
                */
                ErrorMessage = $"Something went wrong during user preset upgrade: {e.Message}";
                IsError = true;
                return Page();
            }
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