import { useEffect, useState } from 'react'
import { call } from '../api'
import SummaryChart from '../components/SummaryChart'
import ExpenseForm from '../components/ExpenseForm'
import ExpenseList from '../components/ExpenseList'

export default function Dashboard() {
  const [items, setItems] = useState([])
  const [editing, setEditing] = useState(null)
  const load = () => call('/expenses').then(setItems)
  useEffect(() => { load() }, [])

  const save = async (data) => {
    if (editing) await call(`/expenses/${editing.id}`, 'PUT', data)
    else await call('/expenses', 'POST', data)
    setEditing(null); load()
  }
  const remove = async (id) => { await call(`/expenses/${id}`, 'DELETE'); load() }

  return (
    <>
      <SummaryChart items={items} />
      <ExpenseForm initial={editing} onSave={save} onCancel={() => setEditing(null)} />
      <ExpenseList items={items} onEdit={setEditing} onDelete={remove} />
    </>
  )
}
