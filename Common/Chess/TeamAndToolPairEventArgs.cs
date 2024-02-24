using Tools;

namespace Common.Chess;

public class TeamAndToolPairEventArgs
{
    public TeamId TeamId { get; }
    public ToolId ToolId { get; }

    public TeamAndToolPairEventArgs(TeamId teamId
                                  , ToolId toolId)
    {
        TeamId = teamId;
        ToolId = toolId;
    }

    public override string ToString()
    {
        return $"{nameof(TeamId)}: {TeamId}, {nameof(ToolId)}: {ToolId}";
    }
}