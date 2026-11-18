import { money } from '../constants'

export default function ExpenseList({ items, onEdit, onDelete }) {
  return (
    <ul className="mt-8 divide-y divide-mist">
      {items.length === 0 && <li className="py-6 text-ink/70">No expenses yet. Add your first one above.</li>}
      {items.map((i) => (
        <li key={i.id} className="flex items-center justify-between py-3">
          <div><p className="font-medium">{i.title}</p><p className="text-sm text-ink/70">{i.category} · {i.date.slice(0, 10)}</p></div>
          <div className="flex items-center gap-4">
            <span>{money(i.amount)}</span>
            <button className="text-sm underline" onClick={() => onEdit(i)}>Edit</button>
            <button className="text-sm text-clay underline" onClick={() => onDelete(i.id)}>Delete</button>
          </div>
        </li>
      ))}
    </ul>
  )
}
