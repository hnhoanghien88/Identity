export async function refreshSession() {
  const response = await fetch('/refresh', {
    method: 'POST',
    credentials: 'include',
  })

  if (!response.ok) {
    throw new Error('Unable to restore the session.')
  }

  return response.json()
}