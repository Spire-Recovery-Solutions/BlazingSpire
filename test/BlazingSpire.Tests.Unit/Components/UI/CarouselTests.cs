using BlazingSpire.Demo.Components.UI;
using BlazingSpire.Tests.Unit.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazingSpire.Tests.Unit.Components.UI;

public class CarouselTests : BlazingSpireTestBase
{
    // ── Carousel ──────────────────────────────────────────────────────────────

    [Fact]
    public void Carousel_Renders_With_Role_Region()
    {
        var cut = Render<Carousel>();
        AssertRole(cut.Find("[role='region']"), "region");
    }

    [Fact]
    public void Carousel_Has_AriaRoleDescription_Carousel()
    {
        var cut = Render<Carousel>();
        Assert.Equal("carousel", cut.Find("[role='region']").GetAttribute("aria-roledescription"));
    }

    [Fact]
    public void Carousel_AdditionalAttributes_PassThrough()
    {
        var cut = Render<Carousel>(p => p.AddUnmatched("data-testid", "carousel"));
        Assert.Equal("carousel", cut.Find("[role='region']").GetAttribute("data-testid"));
    }

    [Fact]
    public void Carousel_ChildContent_Renders()
    {
        var cut = Render<Carousel>(p => p.AddChildContent("<p>Content</p>"));
        Assert.NotNull(cut.Find("[role='region'] p"));
    }

    [Fact]
    public async Task Carousel_NextAsync_Increments_CurrentIndex()
    {
        int received = 0;
        var cut = Render<Carousel>(p => p
            .Add(x => x.CurrentIndex, 0)
            .Add(x => x.CurrentIndexChanged,
                EventCallback.Factory.Create<int>(this, v => received = v)));

        cut.Instance.ItemCount = 3;
        await cut.InvokeAsync(cut.Instance.NextAsync);
        Assert.Equal(1, received);
    }

    [Fact]
    public async Task Carousel_NextAsync_Does_Not_Exceed_ItemCount()
    {
        int received = -1;
        var cut = Render<Carousel>(p => p
            .Add(x => x.CurrentIndex, 2)
            .Add(x => x.CurrentIndexChanged,
                EventCallback.Factory.Create<int>(this, v => received = v)));

        cut.Instance.ItemCount = 3;
        await cut.InvokeAsync(cut.Instance.NextAsync);
        Assert.Equal(-1, received); // callback not invoked
    }

    [Fact]
    public async Task Carousel_PreviousAsync_Decrements_CurrentIndex()
    {
        int received = 0;
        var cut = Render<Carousel>(p => p
            .Add(x => x.CurrentIndex, 2)
            .Add(x => x.CurrentIndexChanged,
                EventCallback.Factory.Create<int>(this, v => received = v)));

        cut.Instance.ItemCount = 3;
        await cut.InvokeAsync(cut.Instance.PreviousAsync);
        Assert.Equal(1, received);
    }

    [Fact]
    public async Task Carousel_PreviousAsync_Does_Not_Go_Below_Zero()
    {
        int received = -1;
        var cut = Render<Carousel>(p => p
            .Add(x => x.CurrentIndex, 0)
            .Add(x => x.CurrentIndexChanged,
                EventCallback.Factory.Create<int>(this, v => received = v)));

        cut.Instance.ItemCount = 3;
        await cut.InvokeAsync(cut.Instance.PreviousAsync);
        Assert.Equal(-1, received); // callback not invoked
    }

    // ── CarouselContent ───────────────────────────────────────────────────────

    [Fact]
    public void CarouselContent_Renders_Inside_Carousel()
    {
        var cut = Render<Carousel>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselContent>(0);
                b.CloseComponent();
            })));

        Assert.NotNull(cut.Find("[role='region'] div"));
    }

    // ── CarouselItem ──────────────────────────────────────────────────────────

    [Fact]
    public void CarouselItem_Has_Role_Group()
    {
        var cut = Render<Carousel>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
            })));

        AssertRole(cut.Find("[role='group']"), "group");
    }

    [Fact]
    public void CarouselItem_Has_AriaRoleDescription_Slide()
    {
        var cut = Render<Carousel>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
            })));

        Assert.Equal("slide", cut.Find("[role='group']").GetAttribute("aria-roledescription"));
    }

    [Fact]
    public void CarouselItem_Registers_With_Carousel()
    {
        var cut = Render<Carousel>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(1);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(2);
                b.CloseComponent();
            })));

        Assert.Equal(3, cut.Instance.ItemCount);
    }

    // ── CarouselPrevious ──────────────────────────────────────────────────────

    [Fact]
    public void CarouselPrevious_Renders_Button()
    {
        var cut = Render<Carousel>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselPrevious>(0);
                b.CloseComponent();
            })));

        Assert.NotNull(cut.Find("button[aria-label='Previous slide']"));
    }

    [Fact]
    public void CarouselPrevious_Disabled_At_First_Slide()
    {
        var cut = Render<Carousel>(p =>
        {
            p.Add(x => x.CurrentIndex, 0);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselPrevious>(0);
                b.CloseComponent();
            }));
        });

        Assert.NotNull(cut.Find("button[disabled]"));
    }

    [Fact]
    public void CarouselPrevious_Not_Disabled_When_Not_At_Start()
    {
        var cut = Render<Carousel>(p =>
        {
            p.Add(x => x.CurrentIndex, 1);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(1);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(2);
                b.CloseComponent();
                b.OpenComponent<CarouselPrevious>(3);
                b.CloseComponent();
            }));
        });

        Assert.Empty(cut.FindAll("button[disabled]"));
    }

    [Fact]
    public async Task CarouselPrevious_Click_Calls_PreviousAsync()
    {
        int received = -1;
        var cut = Render<Carousel>(p =>
        {
            p.Add(x => x.CurrentIndex, 2);
            p.Add(x => x.CurrentIndexChanged,
                EventCallback.Factory.Create<int>(this, v => received = v));
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(1);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(2);
                b.CloseComponent();
                b.OpenComponent<CarouselPrevious>(3);
                b.CloseComponent();
            }));
        });

        await cut.Find("button[aria-label='Previous slide']").ClickAsync(new());
        Assert.Equal(1, received);
    }

    // ── CarouselNext ──────────────────────────────────────────────────────────

    [Fact]
    public void CarouselNext_Renders_Button()
    {
        var cut = Render<Carousel>(p =>
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselNext>(0);
                b.CloseComponent();
            })));

        Assert.NotNull(cut.Find("button[aria-label='Next slide']"));
    }

    [Fact]
    public void CarouselNext_Disabled_At_Last_Slide()
    {
        var cut = Render<Carousel>(p =>
        {
            p.Add(x => x.CurrentIndex, 2);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(1);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(2);
                b.CloseComponent();
                b.OpenComponent<CarouselNext>(3);
                b.CloseComponent();
            }));
        });

        Assert.NotNull(cut.Find("button[disabled]"));
    }

    [Fact]
    public void CarouselNext_Not_Disabled_When_Not_At_End()
    {
        var cut = Render<Carousel>(p =>
        {
            p.Add(x => x.CurrentIndex, 0);
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(1);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(2);
                b.CloseComponent();
                b.OpenComponent<CarouselNext>(3);
                b.CloseComponent();
            }));
        });

        Assert.Empty(cut.FindAll("button[disabled]"));
    }

    [Fact]
    public async Task CarouselNext_Click_Calls_NextAsync()
    {
        int received = -1;
        var cut = Render<Carousel>(p =>
        {
            p.Add(x => x.CurrentIndex, 0);
            p.Add(x => x.CurrentIndexChanged,
                EventCallback.Factory.Create<int>(this, v => received = v));
            p.Add(x => x.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<CarouselItem>(0);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(1);
                b.CloseComponent();
                b.OpenComponent<CarouselItem>(2);
                b.CloseComponent();
                b.OpenComponent<CarouselNext>(3);
                b.CloseComponent();
            }));
        });

        await cut.Find("button[aria-label='Next slide']").ClickAsync(new());
        Assert.Equal(1, received);
    }
}
