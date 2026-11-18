import { CATEGORIES, money } from '../constants'

export default function SummaryChart({ items }) {
  const month = new Date().toISOString().slice(0, 7)
  const mine = items.filter((i) => i.date.startsWith(month))
  const total = mine.reduce((s, i) => s + i.amount, 0)
  const byCat = CATEGORIES.map((c) => ({ c, v: mine.filter((i) => i.category === c).reduce((s, i) => s + i.amount, 0) }))
  const max = Math.max(...byCat.map((x) => x.v), 1)
  return (
    <section className="mt-8">
      <p className="text-ink/70">Spent this month</p>
      <p className="font-display text-5xl text-moss">{money(total)}</p>
      <ul className="mt-4 space-y-2">
        {byCat.map(({ c, v }) => (
          <li key={c} className="flex items-center gap-3 text-sm">
            <span className="w-20">{c}</span>
            <span className="h-3 rounded bg-moss" style={{ width: `${(v / max) * 60}%`, minWidth: v ? 4 : 0 }} />
            <span>{money(v)}</span>
          </li>
        ))}
      </ul>
    </section>
  )
}
