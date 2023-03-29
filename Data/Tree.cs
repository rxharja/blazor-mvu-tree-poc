using Microsoft.AspNetCore.Components;

namespace test.Data;

public interface INode<out T> {
    T Value { get; }
    ActivityStatus Stage { get; }
    RenderFragment Fragment { get; }
}

public interface IBranch<out T> : INode<T> {
    IEnumerable<INode<T>> Branches { get; }
}

public record WorkflowBranch(object Value, IEnumerable<INode<object>> Branches) : IBranch<object> {
    public ActivityStatus Stage => Branches switch {
        _ when Branches.All(o => o.Stage is ActivityStatus.Complete) => ActivityStatus.Complete,
        _ when Branches.All(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Pending,
        _ when Branches.Any(o => o.Stage is ActivityStatus.Active) => ActivityStatus.Active,
        _ when Branches.Any(o => o.Stage is ActivityStatus.Pending) => ActivityStatus.Active,
    };

    public RenderFragment Fragment => Branches switch {
        _ when Branches.FirstOrDefault(o => o.Stage is ActivityStatus.Active) is {} a => a.Fragment,
        _ when Branches.FirstOrDefault(o => o.Stage is ActivityStatus.Pending) is {} p => p.Fragment,
        _ when Branches.All(o => o.Stage is ActivityStatus.Pending) => Branches.First().Fragment,
        _ when Branches.All(o => o.Stage is ActivityStatus.Complete) => Branches.First().Fragment,
    };
}

public record struct WorkflowLeaf(object Value, ActivityStatus Stage, RenderFragment Fragment) : INode<object>;

public static class NodeExtensions
{
    public static INode<T>? Find<T>(this INode<T> node, Func<INode<T>, bool> f) => node switch {
        _ when f(node) => node,
        IBranch<T> branch => branch.Branches.Select(b => b.Find(f)).FirstOrDefault(),
        _ => null
    };
}
