import './ThemeToggle.css'

interface ThemeToggleProps {
  isDark: boolean
  onToggle: () => void
}

export function ThemeToggle({ isDark, onToggle }: ThemeToggleProps) {
  return (
    <div className="theme-toggle">
      <span className="theme-toggle__label">{isDark ? 'Dark mode' : 'Light mode'}</span>
      <button
        role="switch"
        aria-checked={isDark}
        aria-label="Toggle dark mode"
        className={`theme-toggle__switch${isDark ? ' theme-toggle__switch--on' : ''}`}
        onClick={onToggle}
      >
        <span className="theme-toggle__thumb" />
      </button>
    </div>
  )
}
