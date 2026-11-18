import { useState } from 'react'
import { call, setSession } from '../api'
import { input, btn } from '../constants'

export default function AuthForm({ onDone }) {
  const [mode, setMode] = useState('login')
  const [f, setF] = useState({ email: '', password: '' })
  const [error, setError] = useState('')
  const submit = async (e) => {
    e.preventDefault(); setError('')
    try { setSession(await call(`/auth/${mode}`, 'POST', f)); onDone() }
    catch (err) { setError(err.message) }
  }
  return (
    <>
      <form onSubmit={submit} className="mt-8 space-y-4">
        <label className="block">Email<input className={input} type="email" required value={f.email} onChange={(e) => setF({ ...f, email: e.target.value })} /></label>
        <label className="block">Password<input className={input} type="password" required minLength={8} value={f.password} onChange={(e) => setF({ ...f, password: e.target.value })} /></label>
        {error && <p role="alert" className="text-clay">{error}</p>}
        <button className={btn + ' w-full'}>{mode === 'login' ? 'Log in' : 'Create account'}</button>
      </form>
      <button className="mt-4 text-sm underline" onClick={() => setMode(mode === 'login' ? 'register' : 'login')}>
        {mode === 'login' ? 'New here? Create an account' : 'Have an account? Log in'}
      </button>
    </>
  )
}
