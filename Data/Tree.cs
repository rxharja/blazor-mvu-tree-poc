using Microsoft.AspNetCore.Components;

namespace test.Data;

public interface INode<out T> {
    T Value { get; }
    ActivityStatus Stage { get; }
    RenderFragment Fragment { get; }
    RenderFragment Render();
    RenderFragment? Render(Func<INode<T>, bool> f);
}

public interface IBranch<out T> : INode<T> {
    IEnumerable<INode<T>> Branches { get; }
}

public record WorkflowBranch(object Value, RenderFragment Fragment, IEnumerable<INode<object>> Branches) : IBranch<object> {
    public ActivityStatus Stage => Branches switch {
        _ when Branches.All(o => o.Stage is ActivityStatus.Complete) => ActivityStatus.Complete,
        _ when Branches.All(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Pending,
        _ when Branches.Any(o => o.Stage is ActivityStatus.Active) => ActivityStatus.Active,
        _ when Branches.Any(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Pending,
    };

    public RenderFragment Render() => ComposeFragments(Branches switch {
            _ when Branches.FirstOrDefault(o => o.Stage is ActivityStatus.Active) is {} a => a.Render(),
            _ when Branches.FirstOrDefault(o => o.Stage is ActivityStatus.Pending) is {} p => p.Render(),
            _ when Branches.All(o => o.Stage is ActivityStatus.Pending) ||
                Branches.All(o => o.Stage is ActivityStatus.Complete) => Branches.First().Render(),
    });
    
    public RenderFragment? Render(Func<INode<object>, bool> f) => f(this) ? Render() : Branches
        .Select(n => n.Render(f))
        .FirstOrDefault(n => n is not null) is {} r ? ComposeFragments(r) : null;

    RenderFragment ComposeFragments(RenderFragment subFragment) => builder => {
        builder.AddContent(1, Fragment);
        builder.AddContent(2, subFragment);
    };
}

public record WorkflowLeaf(object Value, ActivityStatus Stage, RenderFragment Fragment) : INode<object> {
    public RenderFragment Render() => Fragment;
    public RenderFragment? Render(Func<INode<object>, bool> f) => f(this) ? Fragment : null;
}

public static class Extensions {
    public static INode<T>? Find<T>(this INode<T>? node, Func<INode<T>, bool> f) => node switch {
        IBranch<T> b => f(b) ? b : b.Branches.Select( n => n.Find(f) ).FirstOrDefault(n => n is {}),
        not null => f(node) ? node : null,
        null => null
    };
}