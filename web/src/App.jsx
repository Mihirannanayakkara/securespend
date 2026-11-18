import { useState } from 'react'
import { setSession, hasToken, getRole } from './api'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Admin from './pages/Admin'

export default function App() {
  const [authed, setAuthed] = useState(hasToken())
  const [view, setView] = useState('dashboard')
  if (!authed) return <Login onDone={() => setAuthed(true)} />
  return (
    <main className="mx-auto max-w-3xl px-4 py-10">
      <header className="flex items-center justify-between">
        <h1 className="font-display text-3xl">SecureSpend</h1>
        <nav className="flex gap-4 text-sm">
          {getRole() === 'Admin' && (
            <button className="underline" onClick={() => setView(view === 'admin' ? 'dashboard' : 'admin')}>
              {view === 'admin' ? 'My expenses' : 'Admin'}
            </button>
          )}
          <button className="underline" onClick={() => { setSession(null); setAuthed(false); setView('dashboard') }}>Log out</button>
        </nav>
      </header>
      {view === 'admin' ? <Admin /> : <Dashboard />}
    </main>
  )
}
