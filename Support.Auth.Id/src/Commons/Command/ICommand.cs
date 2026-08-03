namespace Support.Auth.Id.Commons.Command;

// using for need returned data
public interface ICommand<out TResult> { }

// void data scenario
public interface ICommand : ICommand<TUnit> { }

public record struct TUnit;
