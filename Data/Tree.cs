using Microsoft.AspNetCore.Components;

namespace test.Data;

public interface INode<out T> {
    T Value { get; }
    ActivityStatus Stage { get; }
    INode<T>? Find(Func<INode<T>, bool> f);
    RenderFragment Fragment { get; }
    RenderFragment Render();
    RenderFragment? Render(Func<INode<T>, bool> f);
    TOut TraverseActive<TOut>(Func<INode<IProjectItem>, TOut> f);
}

public interface IBranch<out T> : INode<T> {
    IEnumerable<INode<T>> Branches { get; }
}

public record WorkflowBranch(IProjectItem Value, RenderFragment Fragment, IEnumerable<INode<IProjectItem>> Branches) : IBranch<IProjectItem> {
    public ActivityStatus Stage => Branches switch {
        _ when Branches.All(o => o.Stage is ActivityStatus.Complete) => ActivityStatus.Complete,
        _ when Branches.All(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Pending,
        _ when Branches.Any(o => o.Stage is ActivityStatus.Active) => ActivityStatus.Active,
        _ when Branches.Any(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Pending,
    };
    
    public INode<IProjectItem>? Find(Func<INode<IProjectItem>, bool> f) =>
        f(this) ? this : Branches.Select(n => n.Find(f)).FirstOrDefault(n => n is {});

    public T TraverseActive<T>(Func<INode<IProjectItem>, T> f) => Branches switch {
        _ when Branches.FirstOrDefault(o => o.Stage is ActivityStatus.Active) is {} a => f(a),
        _ when Branches.FirstOrDefault(o => o.Stage is ActivityStatus.Pending) is {} p => f(p),
        _ when Branches.All(o => o.Stage is ActivityStatus.Pending) ||
               Branches.All(o => o.Stage is ActivityStatus.Complete) => f(Branches.First()),
    };
    
    public RenderFragment Render() => ComposeFragments(TraverseActive( n => n.Render() ) );
    
    public RenderFragment? Render(Func<INode<IProjectItem>, bool> f) => f(this) ? Render() : Branches
        .Select(n => n.Render(f))
        .FirstOrDefault(n => n is not null) is {} r ? ComposeFragments(r) : null;

    RenderFragment ComposeFragments(RenderFragment subFragment) => builder => {
        builder.AddContent(1, Fragment);
        builder.AddContent(2, subFragment);
    };
}

public record WorkflowLeaf(IProjectItem Value, ActivityStatus Stage, RenderFragment Fragment) : INode<IProjectItem> {
    public INode<IProjectItem>? Find(Func<INode<IProjectItem>, bool> f) => f(this) ? this : null;
    public RenderFragment Render() => Fragment;
    public RenderFragment? Render(Func<INode<IProjectItem>, bool> f) => f(this) ? Fragment : null;
    public TOut TraverseActive<TOut>(Func<INode<IProjectItem>, TOut> f) => f(this);
}