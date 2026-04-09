import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'
import { Sidebar } from './components/Sidebar/Sidebar'
import { NoteForm } from './components/NoteForm/NoteForm'
import { useNotes } from './hooks/useNotes'
import type { Note } from './types/Note'
import './App.css'

type View = { mode: 'add' } | { mode: 'edit'; note: Note } | null

export default function App() {
  const [isDark, setIsDark] = useState(() => localStorage.getItem('theme') === 'dark')
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [view, setView] = useState<View>(null)

  const { notes, loading, createNote, updateNote, deleteNote } = useNotes()

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', isDark ? 'dark' : 'light')
    localStorage.setItem('theme', isDark ? 'dark' : 'light')
  }, [isDark])

  const handleAddNew = () => {
    setView({ mode: 'add' })
    setSidebarOpen(false)
  }

  const handleSelectNote = (note: Note) => {
    setView({ mode: 'edit', note })
    setSidebarOpen(false)
  }

  const handleCreate = async (title: string, content: string) => {
    await createNote({ title, content })
    toast.success('Note created!')
    setView(null)
  }

  const handleUpdate = async (title: string, content: string) => {
    if (view?.mode !== 'edit') return
    const updated = await updateNote(view.note.id, { title, content })
    toast.success('Note saved!')
    setView({ mode: 'edit', note: updated })
  }

  const handleDelete = async () => {
    if (view?.mode !== 'edit') return
    await deleteNote(view.note.id)
    toast.success('Note deleted!')
    setView(null)
  }

  return (
    <div className="app-layout">
      <button
        className="hamburger-btn"
        onClick={() => setSidebarOpen(o => !o)}
        aria-label="Toggle menu"
      >
        <span />
        <span />
        <span />
      </button>

      {sidebarOpen && (
        <div className="sidebar-overlay" onClick={() => setSidebarOpen(false)} />
      )}

      <Sidebar
        notes={notes}
        loading={loading}
        isOpen={sidebarOpen}
        isDark={isDark}
        activeNoteId={view?.mode === 'edit' ? view.note.id : undefined}
        onAddNew={handleAddNew}
        onSelectNote={handleSelectNote}
        onToggleTheme={() => setIsDark(d => !d)}
      />

      <main className="main-content">
        {view === null && (
          <div className="empty-state">
            <p>Select a note or create a new one.</p>
          </div>
        )}

        {view?.mode === 'add' && (
          <NoteForm
            key="add"
            mode="add"
            onSave={handleCreate}
          />
        )}

        {view?.mode === 'edit' && (
          <NoteForm
            key={view.note.id}
            mode="edit"
            note={view.note}
            onSave={handleUpdate}
            onDelete={handleDelete}
          />
        )}
      </main>
    </div>
  )
}
