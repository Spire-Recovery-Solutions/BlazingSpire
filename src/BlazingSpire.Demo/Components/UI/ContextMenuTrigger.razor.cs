using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazingSpire.Demo.Components.Shared;

namespace BlazingSpire.Demo.Components.UI;

public partial class ContextMenuTrigger : ChildOf<ContextMenu>
{
    public ContextMenu? ParentMenu => Parent;

    protected override string BaseClasses => "inline-block";

    private async Task OnContextMenuAsync(MouseEventArgs e)
    {
        if (ParentMenu is not null)
            await ParentMenu.SetIsOpenAsync(true);
    }
}
