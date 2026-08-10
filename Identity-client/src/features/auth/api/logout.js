export async function logout(accessToken) {
  const response = await fetch('/api/logout', {
    method: 'POST',
    credentials: 'include',
    headers: {
      Authorization: 'Bearer ' + accessToken,
    },
  })

  if (!response.ok && response.status !== 401) {
    throw new Error('Unable to log out.')
  }
}