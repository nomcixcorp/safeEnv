const API_URL = import.meta.env.VITE_API_URL ?? ''

export async function generateValues(payload) {
  const response = await fetch(`${API_URL}/api/generate`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(payload),
  })

  if (!response.ok) {
    let errorMessage = 'Failed to generate values.'
    try {
      const body = await response.json()
      if (body?.error) {
        errorMessage = body.error
      }
    } catch {
      // keep generic error when response body is not JSON
    }

    throw new Error(errorMessage)
  }

  return response.json()
}
