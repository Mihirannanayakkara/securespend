import AuthForm from '../components/AuthForm'

export default function Login({ onDone }) {
  return (
    <main className="mx-auto mt-24 max-w-sm px-4">
      <h1 className="font-display text-4xl">SecureSpend</h1>
      <p className="mt-2 text-ink/70">Track what you spend. Your data stays yours.</p>
      <AuthForm onDone={onDone} />
    </main>
  )
}
