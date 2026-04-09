import { useCallback, useEffect, useState } from 'react'
import { notesApi } from '../services/notesApi'
import type { CreateNoteRequest, Note, UpdateNoteRequest } from '../types/Note'

export function useNotes() {
  const [notes, setNotes] = useState<Note[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const fetchNotes = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await notesApi.getAll()
      setNotes(data)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load notes')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    fetchNotes()
  }, [fetchNotes])

  const createNote = async (data: CreateNoteRequest): Promise<Note> => {
    const note = await notesApi.create(data)
    setNotes(prev => [...prev, note].sort(
      (a, b) => new Date(a.createdAtUtc).getTime() - new Date(b.createdAtUtc).getTime(),
    ))
    return note
  }

  const updateNote = async (id: string, data: UpdateNoteRequest): Promise<Note> => {
    const updated = await notesApi.update(id, data)
    setNotes(prev => prev.map(n => (n.id === id ? updated : n)))
    return updated
  }

  const deleteNote = async (id: string): Promise<void> => {
    await notesApi.delete(id)
    setNotes(prev => prev.filter(n => n.id !== id))
  }

  return { notes, loading, error, fetchNotes, createNote, updateNote, deleteNote }
}
