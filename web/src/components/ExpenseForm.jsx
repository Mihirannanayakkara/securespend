import { useEffect, useState } from 'react'
import { CATEGORIES, input, btn } from '../constants'

const blank = () => ({ title: '', amount: '', category: CATEGORIES[0], date: new Date().toISOString().slice(0, 10) })

export default function ExpenseForm({ initial, onSave, onCancel }) {
  const [f, setF] = useState(blank())
  const [error, setError] = useState('')
  useEffect(() => { setF(initial ? { ...initial, date: initial.date.slice(0, 10) } : blank()) }, [initial])

  const submit = async (e) => {
    e.preventDefault(); setError('')
    try { await onSave({ ...f, amount: Number(f.amount), date: new Date(f.date).toISOString() }) }
    catch (err) { setError(err.message) }
  }
  return (
    <form onSubmit={submit} className="mt-10 grid gap-3 sm:grid-cols-5">
      <input className={input + ' sm:col-span-2'} placeholder="What was it?" required value={f.title} onChange={(e) => setF({ ...f, title: e.target.value })} />
      <input className={input} type="number" step="0.01" min="0.01" placeholder="Amount" required value={f.amount} onChange={(e) => setF({ ...f, amount: e.target.value })} />
      <select className={input} value={f.category} onChange={(e) => setF({ ...f, category: e.target.value })}>{CATEGORIES.map((c) => <option key={c}>{c}</option>)}</select>
      <input className={input} type="date" required value={f.date} onChange={(e) => setF({ ...f, date: e.target.value })} />
      <button className={btn + (initial ? ' sm:col-span-3' : ' sm:col-span-5')}>{initial ? 'Save changes' : 'Add expense'}</button>
      {initial && <button type="button" className="rounded border border-mist px-4 py-2 sm:col-span-2" onClick={onCancel}>Cancel</button>}
      {error && <p role="alert" className="text-clay sm:col-span-5">{error}</p>}
    </form>
  )
}
