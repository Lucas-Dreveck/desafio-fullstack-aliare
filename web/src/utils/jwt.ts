export interface JwtPayload {
  sub: string
  unique_name: string
  email: string
  exp: number
  iss: string
  aud: string
}

function decodeBase64Url(base64Url: string): string {
  const base64: string = base64Url.replaceAll('-', '+').replaceAll('_', '/')
  return decodeURIComponent(
    atob(base64)
      .split('')
      .map((char: string) => '%' + ('00' + (char.codePointAt(0) ?? 0).toString(16)).slice(-2))
      .join(''),
  )
}

export function decodeJwt(token: string): JwtPayload | null {
  try {
    const parts: string[] = token.split('.')
    if (parts.length !== 3) return null

    const encodedPayload: string | undefined = parts[1]
    if (!encodedPayload) return null

    const payload: string = decodeBase64Url(encodedPayload)
    return JSON.parse(payload) as JwtPayload
  } catch {
    return null
  }
}

export function isTokenExpired(token: string): boolean {
  const payload: JwtPayload | null = decodeJwt(token)
  if (!payload) return true

  const now: number = Math.floor(Date.now() / 1000)
  return payload.exp < now
}
