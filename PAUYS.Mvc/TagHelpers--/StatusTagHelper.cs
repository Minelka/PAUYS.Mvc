using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PAUYS.AspNetCoreMvc.TagHelpers
{
    public class StatusTagHelper : TagHelper
    {
        public bool Status { get; set; }

        public string TrueText { get; set; } = null!;

        public string FalseText { get; set; } = null!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            if (Status)
            {
                output.Attributes.SetAttribute("class", "text-success");
                output.Content.SetContent(TrueText);
            }
            else
            {
                output.Attributes.SetAttribute("class", "text-danger");
                output.Content.SetContent(FalseText);
            }
        }
    }
}
