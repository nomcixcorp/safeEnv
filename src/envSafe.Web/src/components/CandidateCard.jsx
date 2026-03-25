import FormatRow from './FormatRow'

const formatOrder = [
  ['Raw value', 'rawValue'],
  ['.env format', 'outputs.env'],
  ['Bash export', 'outputs.bash'],
  ['PowerShell', 'outputs.powerShell'],
  ['JSON property', 'outputs.json'],
  ['YAML key/value', 'outputs.yaml'],
  ['Docker compose env', 'outputs.dockerCompose'],
]

function getByPath(obj, path) {
  return path.split('.').reduce((current, key) => current?.[key], obj)
}

export default function CandidateCard({ candidate, index, onCopy, copiedKey }) {
  const cardId = `candidate-${index + 1}`

  return (
    <article className="rounded-xl border border-zinc-200 bg-white p-5 shadow-sm">
      <header className="mb-4 flex flex-wrap items-center justify-between gap-3 border-b border-zinc-100 pb-3">
        <div>
          <h3 className="text-base font-semibold text-zinc-900">Candidate {index + 1}</h3>
          <p className="text-xs text-zinc-500">{candidate.whyThisIsSafe}</p>
        </div>
        <div className="text-xs text-zinc-600">
          {candidate.metadata.wasRegenerated ? 'Regenerated for safety' : 'Generated first pass'}
        </div>
      </header>

      <div className="space-y-2">
        {formatOrder.map(([label, path]) => {
          const value = getByPath(candidate, path)
          const copyKey = `${cardId}:${path}`
          const copied = copiedKey === copyKey
          return (
            <FormatRow
              key={path}
              label={label}
              value={value}
              onCopy={() => onCopy(value, copyKey)}
              copied={copied}
            />
          )
        })}
      </div>

      <footer className="mt-4 border-t border-zinc-100 pt-3 text-xs text-zinc-600">
        <p>Flags: {candidate.metadata.flags.length > 0 ? candidate.metadata.flags.join(' • ') : 'none'}</p>
        <p className="mt-1">
          Escaping applied: {Object.entries(candidate.metadata.escapingApplied)
            .filter(([, escaped]) => escaped)
            .map(([format]) => format)
            .join(', ') || 'none'}
        </p>
      </footer>
    </article>
  )
}
