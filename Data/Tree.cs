using Microsoft.AspNetCore.Components;

namespace test.Data;

public interface INode<out T> {
    T Value { get; }
    ActivityStatus Stage { get; }
    RenderFragment? Fragment { get; }
    INode<T>? Find(Func<INode<T>, bool> f);
    public INode<T>? FindNext(Func<INode<T>, bool> f);
    public TOut TraverseActive<TOut>(Func<INode<T>, TOut> f);
}

public record WorkflowBranch(IProjectItem Value, RenderFragment Fragment, IEnumerable<INode<IProjectItem>> Branches) : INode<IProjectItem> {
    public ActivityStatus Stage => Branches switch {
        _ when Branches.All(o => o.Stage is ActivityStatus.Complete) => ActivityStatus.Complete,
        _ when Branches.All(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Pending,
        _ when Branches.Any(o => o.Stage > ActivityStatus.Pending ) => ActivityStatus.Active,
        _ when Branches.Any(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Pending,
    };
    public INode<IProjectItem>? Find(Func<INode<IProjectItem>, bool> f) => 
        f(this) ? this : Branches.Select(n => n.Find(f)).FirstOrDefault(n => n is {});
    public INode<IProjectItem>? FindNext(Func<INode<IProjectItem>, bool> f) => 
        f(this) ? this : Branches.FirstOrDefault(n => n.Find(f) is {});
    
    public T TraverseActive<T>(Func<INode<IProjectItem>, T> f) => Branches switch {
        _ when Branches.FirstOrDefault(o => o.Stage is ActivityStatus.Active or ActivityStatus.Pending) is {} a => f(a),
        _ when Branches.All(o => o.Stage is ActivityStatus.Pending) ||
            Branches.All(o => o.Stage is ActivityStatus.Complete) => f(Branches.First()),
    };
}

public record WorkflowLeaf(IProjectItem Value, ActivityStatus Stage, RenderFragment? Fragment) : INode<IProjectItem> {
    public INode<IProjectItem>? Find(Func<INode<IProjectItem>, bool> f) => f(this) ? this : null;
    public INode<IProjectItem>? FindNext(Func<INode<IProjectItem>, bool> f) => Find(f);
    public TOut TraverseActive<TOut>(Func<INode<IProjectItem>, TOut> f) => default;
}

public static class Render {
    public static RenderFragment? ActivePath<T>(INode<T> node) => node.Fragment + node.TraverseActive(ActivePath);
    public static RenderFragment? PathOf<T>(INode<T> node, Func<INode<T>, bool> f) => node.Fragment + ActivePath(node.FindNext(f) ?? node);
}
