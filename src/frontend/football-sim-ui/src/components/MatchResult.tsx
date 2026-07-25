import type { MatchResultResponse, ScoreDto } from '../api/footballApi';

interface Props {
  result: MatchResultResponse;
}

export function MatchResult({ result }: Props) {
  const { homeTeam, awayTeam, regularTime, extraTime, penalties, finalScore, winner } = result;
  const isDraw = winner === 'Draw';

  return (
    <div className="result-card">
      <div className="result-header">
        <span className="result-team home">{homeTeam.name}</span>
        <div className="result-score-block">
          <span className="result-score">{finalScore.homeScore}</span>
          <span className="result-dash">–</span>
          <span className="result-score">{finalScore.awayScore}</span>
        </div>
        <span className="result-team away">{awayTeam.name}</span>
      </div>

      <div className="result-winner">
        {isDraw ? '🤝 Draw' : `🏆 ${winner} wins`}
      </div>

      <div className="result-breakdown">
        <ScoreRow label="Regular Time" score={regularTime} />
        {extraTime && <ScoreRow label="Extra Time" score={extraTime} highlight />}
        {penalties && <ScoreRow label="Penalties" score={penalties} highlight />}
      </div>
    </div>
  );
}

function ScoreRow({
  label,
  score,
  highlight = false,
}: {
  label: string;
  score: ScoreDto;
  highlight?: boolean;
}) {
  return (
    <div className={`score-row ${highlight ? 'score-row--highlight' : ''}`}>
      <span className="score-row-label">{label}</span>
      <span className="score-row-value">
        {score.homeScore} – {score.awayScore}
      </span>
    </div>
  );
}
