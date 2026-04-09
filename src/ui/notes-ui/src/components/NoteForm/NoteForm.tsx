import { useState } from 'react'
import type { Note } from '../../types/Note'
import './NoteForm.css'

const TITLE_MAX = 150
const CONTENT_MAX = 2000

interface NoteFormProps {
  mode: 'add' | 'edit'
  note?: Note
  onSave: (title: string, content: string) => Promise<void>
  onDelete?: () => Promise<void>
}

export function NoteForm({ mode, note, onSave, onDelete }: NoteFormProps) {
  const [title, setTitle] = useState(note?.title ?? '')
  const [content, setContent] = useState(note?.content ?? '')
  const [saving, setSaving] = useState(false)
  const [deleting, setDeleting] = useState(false)

  const titleError =
    title.trim().length === 0
      ? 'Title is required.'
      : title.trim().length > TITLE_MAX
        ? `Title must be at most ${TITLE_MAX} characters.`
        : ''

  const contentError =
    content.trim().length === 0
      ? 'Content is required.'
      : content.trim().length > CONTENT_MAX
        ? `Content must be at most ${CONTENT_MAX} characters.`
        : ''

  const isValid = !titleError && !contentError

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!isValid || saving) return
    setSaving(true)
    try {
      await onSave(title.trim(), content.trim())
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    if (!onDelete || deleting) return
    setDeleting(true)
    try {
      await onDelete()
    } finally {
      setDeleting(false)
    }
  }

  return (
    <form className="note-form" onSubmit={handleSubmit} noValidate>
      <h2 className="note-form__heading">
        {mode === 'add' ? 'New Note' : 'Edit Note'}
      </h2>

      <div className="note-form__field">
        <label htmlFor="note-title" className="note-form__label">
          Title
        </label>
        <input
          id="note-title"
          type="text"
          className={`note-form__input${titleError && title.length > 0 ? ' note-form__input--error' : ''}`}
          value={title}
          onChange={e => setTitle(e.target.value)}
          placeholder="Note title…"
          maxLength={TITLE_MAX + 10}
        />
        <div className="note-form__meta">
          {titleError && title.length > 0 && (
            <span className="note-form__error">{titleError}</span>
          )}
          <span className="note-form__counter">
            {title.trim().length}/{TITLE_MAX}
          </span>
        </div>
      </div>

      <div className="note-form__field note-form__field--grow">
        <label htmlFor="note-content" className="note-form__label">
          Content
        </label>
        <textarea
          id="note-content"
          className={`note-form__textarea${contentError && content.length > 0 ? ' note-form__input--error' : ''}`}
          value={content}
          onChange={e => setContent(e.target.value)}
          placeholder="Write your note…"
          maxLength={CONTENT_MAX + 10}
        />
        <div className="note-form__meta">
          {contentError && content.length > 0 && (
            <span className="note-form__error">{contentError}</span>
          )}
          <span className="note-form__counter">
            {content.trim().length}/{CONTENT_MAX}
          </span>
        </div>
      </div>

      <div className="note-form__actions">
        <button
          type="submit"
          className="note-form__btn note-form__btn--primary"
          disabled={!isValid || saving}
        >
          {saving ? 'Saving…' : mode === 'add' ? 'Create' : 'Save'}
        </button>

        {mode === 'edit' && onDelete && (
          <button
            type="button"
            className="note-form__btn note-form__btn--danger"
            onClick={handleDelete}
            disabled={deleting}
            aria-label="Delete note"
            title="Delete note"
          >
            {deleting ? '…' : (
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="3 6 5 6 21 6" />
                <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                <path d="M10 11v6M14 11v6" />
                <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
              </svg>
            )}
          </button>
        )}
      </div>
    </form>
  )
}
