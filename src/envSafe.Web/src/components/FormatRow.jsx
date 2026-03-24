export default function FormatRow({ label, value, onCopy, copied }) {
  return (
    <div className="rounded-md border border-slate-200 bg-slate-50 p-3">
      <div className="mb-2 flex items-center justify-between gap-3">
        <p className="text-xs font-semibold uppercase tracking-wide text-slate-600">
          {label}
        </p>
        <button
          type="button"
          onClick={onCopy}
          className="rounded-md border border-slate-300 bg-white px-2 py-1 text-xs font-medium text-slate-700 transition hover:border-slate-400 hover:bg-slate-100"
        >
          {copied ? 'Copied' : 'Copy'}
        </button>
      </div>
      <pre className="overflow-x-auto whitespace-pre-wrap break-all text-xs text-slate-800">
        {value}
      </pre>
    </div>
  )
}
