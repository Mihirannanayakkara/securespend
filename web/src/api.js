export const setSession = (s) => {
  if (s) { sessionStorage.setItem('token', s.token); sessionStorage.setItem('role', s.role) }
  else { sessionStorage.removeItem('token'); sessionStorage.removeItem('role') }
}
export const hasToken = () => !!sessionStorage.getItem('token')
export const getRole = () => sessionStorage.getItem('role')

export async function call(path, method = 'GET', body) {
  const token = sessionStorage.getItem('token')
  const res = await fetch(`/api${path}`, {
    method,
    headers: { 'Content-Type': 'application/json', ...(token && { Authorization: `Bearer ${token}` }) },
    body: body ? JSON.stringify(body) : undefined,
  })
  if (res.status === 401 && token) { setSession(null); location.reload() }
  if (!res.ok) {
    const err = await res.json().catch(() => ({}))
    throw new Error(err.error || (res.status === 429 ? 'Too many attempts. Wait a minute and try again.' : 'Something went wrong.'))
  }
  return res.status === 204 ? null : res.json()
}
