import { useMemo, useState } from 'react'
import CandidateCard from './components/CandidateCard'
import { generateValues } from './lib/api'

const VALUE_TYPES = [
  { value: 'Password', label: 'Password' },
  { value: 'ApiKey', label: 'API Key' },
  { value: 'Token', label: 'Token' },
  { value: 'UrlSafeString', label: 'URL-safe string' },
  { value: 'ConnectionStringSafeValue', label: 'Connection-string-safe value' },
]

const MODES = [
  {
    value: 'StrictSafe',
    label: 'Strict Safe',
    help: 'Alphanumeric only for maximum cross-tool compatibility.',
  },
  {
    value: 'Balanced',
    label: 'Balanced',
    help: 'Mostly safe values with a conservative symbol set.',
  },
  {
    value: 'MaxEntropy',
    label: 'Max Entropy',
    help: 'Broader printable set, excluding obvious copy/paste hazards.',
  },
]

const initialForm = {
  variableName: 'ENVSAFE_SECRET',
  valueType: 'Token',
  mode: 'Balanced',
  length: 32,
}

function App() {
  const [form, setForm] = useState(initialForm)
  const [candidates, setCandidates] = useState([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [copiedKey, setCopiedKey] = useState('')

  const modeHelp = useMemo(
    () => MODES.find((mode) => mode.value === form.mode)?.help ?? '',
    [form.mode],
  )

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({
      ...current,
      [name]: name === 'length' ? Number(value) : value,
    }))
  }

  const handleGenerate = async (event) => {
    event?.preventDefault()
    setLoading(true)
    setError('')

    try {
      const response = await generateValues(form)
      setCandidates(response.candidates ?? [])
    } catch (requestError) {
      setCandidates([])
      setError(requestError.message)
    } finally {
      setLoading(false)
    }
  }

  const handleCopy = async (value, key) => {
    try {
      await navigator.clipboard.writeText(value)
      setCopiedKey(key)
      window.setTimeout(() => {
        setCopiedKey((current) => (current === key ? '' : current))
      }, 1200)
    } catch {
      setError('Copy failed. Please copy manually.')
    }
  }

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <main className="mx-auto flex w-full max-w-6xl flex-col gap-6 px-4 py-8 md:px-8 md:py-12">
        <header className="space-y-3">
          <p className="text-sm font-medium uppercase tracking-[0.18em] text-cyan-300">
            envSafe MVP
          </p>
          <h1 className="text-3xl font-semibold leading-tight text-white md:text-4xl">
            Generate safer environment variable values
          </h1>
          <p className="max-w-3xl text-sm text-slate-300 md:text-base">
            envSafe helps generate random values and format them safely for common
            config files and shells. It is not a secrets manager or vault.
          </p>
        </header>

        <section className="rounded-2xl border border-slate-800 bg-slate-900/80 p-4 shadow-xl shadow-slate-950/40 md:p-6">
          <form className="grid gap-4 md:grid-cols-2" onSubmit={handleGenerate}>
            <label className="flex flex-col gap-2 text-sm">
              <span className="text-slate-300">Variable name</span>
              <input
                className="rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm text-slate-100 outline-none transition focus:border-cyan-500"
                name="variableName"
                value={form.variableName}
                onChange={handleChange}
                placeholder="ENVSAFE_SECRET"
                required
              />
            </label>

            <label className="flex flex-col gap-2 text-sm">
              <span className="text-slate-300">Value type</span>
              <select
                className="rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm text-slate-100 outline-none transition focus:border-cyan-500"
                name="valueType"
                value={form.valueType}
                onChange={handleChange}
              >
                {VALUE_TYPES.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </label>

            <label className="flex flex-col gap-2 text-sm">
              <span className="text-slate-300">Mode</span>
              <select
                className="rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm text-slate-100 outline-none transition focus:border-cyan-500"
                name="mode"
                value={form.mode}
                onChange={handleChange}
              >
                {MODES.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
              <span className="text-xs text-slate-400">{modeHelp}</span>
            </label>

            <label className="flex flex-col gap-2 text-sm">
              <span className="text-slate-300">Length</span>
              <input
                className="rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm text-slate-100 outline-none transition focus:border-cyan-500"
                name="length"
                type="number"
                min="8"
                max="256"
                value={form.length}
                onChange={handleChange}
              />
            </label>

            <div className="flex flex-wrap items-center gap-3 md:col-span-2">
              <button
                type="submit"
                className="rounded-lg bg-cyan-500 px-4 py-2 text-sm font-semibold text-slate-950 transition hover:bg-cyan-400 disabled:cursor-not-allowed disabled:bg-cyan-800"
                disabled={loading}
              >
                {loading ? 'Generating…' : 'Generate 3 values'}
              </button>
              <button
                type="button"
                className="rounded-lg border border-slate-700 px-4 py-2 text-sm font-medium text-slate-200 transition hover:border-slate-500 hover:text-white"
                onClick={handleGenerate}
                disabled={loading}
              >
                Regenerate
              </button>
              {error ? <p className="text-sm text-rose-300">{error}</p> : null}
            </div>
          </form>
        </section>

        <p className="rounded-lg border border-slate-800 bg-slate-900/60 px-4 py-3 text-xs text-slate-400 md:text-sm">
          envSafe produces values that are safer across common formats, but you
          should still validate compatibility with your own systems and runtime.
        </p>

        <section className="grid gap-4">
          {candidates.length === 0 ? (
            <div className="rounded-xl border border-dashed border-slate-700 bg-slate-900/40 p-6 text-sm text-slate-400">
              Generate values to see raw output, format-ready snippets, and safety
              explanations.
            </div>
          ) : (
            candidates.map((candidate, index) => (
              <CandidateCard
                key={`${candidate.rawValue}-${index}`}
                candidate={candidate}
                index={index}
                onCopy={handleCopy}
                copiedKey={copiedKey}
              />
            ))
          )}
        </section>
      </main>
    </div>
  )
}

export default App
