using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace WebDemo.Api.Options
{
    public class TestSnapModel : PageModel
    {
        private readonly PositionOption _snapshotOptions;

        public TestSnapModel(IOptionsSnapshot<PositionOption> snapshotOptionsAccessor)
        {
            _snapshotOptions = snapshotOptionsAccessor.Value;
        }

        public ContentResult OnGet()
        {
            return Content($"Title: {_snapshotOptions.Title} \n" +
                           $"Name: {_snapshotOptions.Name}");
        }
    }
}
