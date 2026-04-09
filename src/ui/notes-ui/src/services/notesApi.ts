import type { CreateNoteRequest, Note, UpdateNoteRequest } from '../types/Note'

const BASE = '/api/notes'

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text().catch(() => res.statusText)
    throw new Error(text || `HTTP ${res.status}`)
  }
  if (res.status === 204) return undefined as T
  return res.json() as Promise<T>
}

export const notesApi = {
  getAll(): Promise<Note[]> {
    return fetch(BASE).then(res => handleResponse<Note[]>(res))
  },

  create(data: CreateNoteRequest): Promise<Note> {
    return fetch(BASE, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    }).then(res => handleResponse<Note>(res))
  },

  update(id: string, data: UpdateNoteRequest): Promise<Note> {
    return fetch(`${BASE}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    }).then(res => handleResponse<Note>(res))
  },

  delete(id: string): Promise<void> {
    return fetch(`${BASE}/${id}`, { method: 'DELETE' }).then(res =>
      handleResponse<void>(res),
    )
  },
}
