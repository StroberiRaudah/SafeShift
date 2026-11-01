using SafeShift.Api.Domain;

namespace SafeShift.Api.Tests;

public class CorrectiveActionTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    [Theory]
    [InlineData(-1, ActionStatus.Open, true)]         // due yesterday, still open
    [InlineData(-1, ActionStatus.InProgress, true)]
    [InlineData(0, ActionStatus.Open, false)]         // due today is not overdue yet
    [InlineData(3, ActionStatus.Open, false)]
    [InlineData(-5, ActionStatus.Done, false)]        // finished actions are never overdue
    public void IsOverdue_only_when_past_due_and_not_done(int dueInDays, ActionStatus status, bool expected)
    {
        var action = new CorrectiveAction { DueDate = Today.AddDays(dueInDays), Status = status };

        Assert.Equal(expected, action.IsOverdue(Today));
    }
}
