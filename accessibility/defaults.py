"""
Spiny Flannel Society — Default Values
Reference table of what each default means under Standard Defaults
vs after rewriting.

This file is purely informational / documentation-as-code.
The actual values live in core/defaults_registry.py.
"""

# ─── Quick Reference ────────────────────────────────────────────────
#
# KEY                      RIGID (Drift)    REWRITTEN (Restored)
# ─────────────────────── ──────────────── ────────────────────
# timing_window            0.2  (200 ms)    0.5  (500 ms)
# platform_rhythm          1.0  (full)      0.6  (60 %)
# coyote_time              0.0  (none)      0.2  (200 ms)
# jump_buffer              0.0  (none)      0.15 (150 ms)
#
# visual_clutter           1.0  (max)       0.4  (breathable)
# audio_layering           1.0  (all)       0.5  (ducked)
# screen_shake             1.0  (full)      0.0  (none)
#
# route_strictness         1.0  (one path)  0.3  (many routes)
# safe_route_visibility    0.0  (hidden)    1.0  (main path)
#
# communication_rigidity   1.0  (one mode)  0.0  (all equal)
# social_script_penalty    1.0  (punished)  0.0  (accepted)
#
# failure_penalty          1.0  (full)      0.1  (gentle)
# retry_cost               1.0  (costly)    0.0  (free)
#
# consent_gates            0.0  (none)      1.0  (present)
# opt_out_available        0.0  (locked in) 1.0  (can leave)
