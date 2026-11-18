import { useEffect, useState } from 'react'
import { call } from '../api'
import { money } from '../constants'

export default function Admin() {
  const [stats, setStats] = useState(null)
  const [users, setUsers] = useState([])
  const [error, setError] = useState('')
  useEffect(() => {
    Promise.all([call('/admin/stats'), call('/admin/users')]).then(([s, u]) => { setStats(s); setUsers(u) }).catch((e) => setError(e.message))
  }, [])
  if (error) return <p role="alert" className="mt-8 text-clay">{error}</p>
  if (!stats) return <p className="mt-8">Loading…</p>
  return (
    <section className="mt-8">
      <p className="text-ink/70">Admins see totals only, never individual expenses.</p>
      <dl className="mt-4 grid grid-cols-3 gap-4">
        <div><dt className="text-sm text-ink/70">Users</dt><dd className="font-display text-3xl">{stats.users}</dd></div>
        <div><dt className="text-sm text-ink/70">Expenses logged</dt><dd className="font-display text-3xl">{stats.expenses}</dd></div>
        <div><dt className="text-sm text-ink/70">Total tracked</dt><dd className="font-display text-3xl">{money(stats.total)}</dd></div>
      </dl>
      <table className="mt-8 w-full text-left text-sm">
        <thead><tr className="border-b border-mist"><th className="py-2">Email</th><th>Role</th><th>Expenses</th></tr></thead>
        <tbody>{users.map((u) => <tr key={u.id} className="border-b border-mist"><td className="py-2">{u.email}</td><td>{u.role}</td><td>{u.expenseCount}</td></tr>)}</tbody>
      </table>
    </section>
  )
}
