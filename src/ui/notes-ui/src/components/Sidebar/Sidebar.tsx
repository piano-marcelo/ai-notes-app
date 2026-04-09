import type { Note } from '../../types/Note'
import { ThemeToggle } from '../ThemeToggle/ThemeToggle'
import './Sidebar.css'

interface SidebarProps {
  notes: Note[]
  loading: boolean
  isOpen: boolean
  isDark: boolean
  activeNoteId?: string
  onAddNew: () => void
  onSelectNote: (note: Note) => void
  onToggleTheme: () => void
}

export function Sidebar({
  notes,
  loading,
  isOpen,
  isDark,
  activeNoteId,
  onAddNew,
  onSelectNote,
  onToggleTheme,
}: SidebarProps) {
  return (
    <aside className={`sidebar${isOpen ? ' sidebar--open' : ''}`}>
      <div className="sidebar__top">
        <button className="sidebar__add-btn" onClick={onAddNew}>
          <span className="sidebar__add-icon">+</span>
          Add new
        </button>
      </div>

      <nav className="sidebar__notes">
        {loading && <p className="sidebar__loading">Loading…</p>}
        {!loading && notes.length === 0 && (
          <p className="sidebar__empty">No notes yet.</p>
        )}
        {notes.map(note => (
          <button
            key={note.id}
            className={`sidebar__note-item${note.id === activeNoteId ? ' sidebar__note-item--active' : ''}`}
            onClick={() => onSelectNote(note)}
          >
            <span className="sidebar__note-title">{note.title}</span>
            <span className="sidebar__note-date">
              {new Date(note.createdAtUtc).toLocaleDateString()}
            </span>
          </button>
        ))}
      </nav>

      <div className="sidebar__bottom">
        <ThemeToggle isDark={isDark} onToggle={onToggleTheme} />
      </div>
    </aside>
  )
}
