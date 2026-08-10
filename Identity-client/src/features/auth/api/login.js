export async function login(credentials) {
  const response = await fetch('/login', {
    method: 'POST',
    credentials: 'include',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  })

  if (!response.ok) {
    throw new Error('Email hoặc mật khẩu không đúng.')
  }

  return response.json()
}