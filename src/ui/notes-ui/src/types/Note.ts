export interface Note {
  id: string
  title: string
  content: string
  createdAtUtc: string
  updatedAtUtc: string | null
  isActive: boolean
}

export interface CreateNoteRequest {
  title: string
  content: string
}

export interface UpdateNoteRequest {
  title: string
  content: string
}
