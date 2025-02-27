using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PAUYS.AspNetCoreMvc.TagHelpers
{
    public class ActionsButtonTagHelper : TagHelper
    {
        private const string EditLinkTextDefault = "Düzenle";
        private const string DetailLinkTextDefault = "Detay";
        private const string DeleteLinkTextDefault = "Sil";

        public string AriaLabel { get; set; } = null!;

        public string EditLinkText { get; set; } = EditLinkTextDefault;
        public string EditLink { get; set; } = null!;    // /Controller/Edit/5

        public string DetailLinkText { get; set; } = DetailLinkTextDefault;
        public string DetailLink { get; set; } = null!; // /Controller/Detail/5

        public string DeleteLinkText { get; set; } = DeleteLinkTextDefault;
        public string DeleteLink { get; set; } = null!;  // /Controller/Delete/5

        public string DeleteModalConfirmMessage { get; set; } = "Kaydı kalıcı olarak silmek istediğinizden emin misiniz?";
        public string DeleteModalId { get; set; } = "deleteItemConfirmModal";
        public bool DeleteModalConfirm { get; set; } = false;

        public List<(string LinkText, string Link)> CustomButtons { get; set; } = new List<(string LinkText, string Link)>();
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "btn-group");
            output.Attributes.SetAttribute("role", "group");
            output.Attributes.SetAttribute("aria-label", AriaLabel);

            TagBuilder aEdit = new TagBuilder("a");
            TagBuilder aDetail = new TagBuilder("a");
            TagBuilder aDelete = new TagBuilder("a");

            aEdit.Attributes.Add("role", "button");
            aEdit.Attributes.Add("class", "btn btn-warning");
            aEdit.Attributes.Add("href", EditLink);
            aEdit.InnerHtml.SetContent(EditLinkText);

            aDetail.Attributes.Add("role", "button");
            aDetail.Attributes.Add("class", "btn btn-secondary");
            aDetail.Attributes.Add("href", DetailLink);
            aDetail.InnerHtml.SetContent(DetailLinkText);

            aDelete.Attributes.Add("role", "button");
            aDelete.Attributes.Add("class", "btn btn-danger");
            aDelete.InnerHtml.SetContent(DeleteLinkText);

            if (!DeleteModalConfirm)
            {
                aDelete.Attributes.Add("href", DeleteLink);
            }
            else 
            {
                aDelete.Attributes.Add("data-href", DeleteLink);
                aDelete.Attributes.Add("data-delete-confirm-message", DeleteModalConfirmMessage);
                aDelete.Attributes.Add("data-delete-modal-id", $"#{DeleteModalId}");
            }


            output.Content.AppendHtml(aEdit);
            output.Content.AppendHtml(aDetail);
            output.Content.AppendHtml(aDelete);

            if (CustomButtons.Any())
            {
                foreach ((string LinkText, string Link)customButton in CustomButtons)
                {
                    TagBuilder aCustomButton = new TagBuilder("a");

                    aCustomButton.Attributes.Add("role", "button");
                    aCustomButton.Attributes.Add("class", "btn btn-primary");
                    aCustomButton.Attributes.Add("href", customButton.Link);
                    aCustomButton.InnerHtml.SetContent(customButton.LinkText);

                    output.Content.AppendHtml(aCustomButton);
                }
            }

        }

    }
}