# server.py
from flask import Flask, request, jsonify
from transformers import pipeline
import torch
from uuid import uuid4
from datetime import datetime
from threading import Lock
import time

app = Flask(__name__)
model_name = "microsoft/Phi-3-mini-4k-instruct"

# Conversation store with session management
conversations = {}
conversation_lock = Lock()
MAX_HISTORY = 100  # Keep last 10 exchanges
SESSION_TIMEOUT = 1800  # 30 minutes in seconds

# Load model once
pipe = pipeline(
    "text-generation",
    model=model_name,
    device=0 if torch.cuda.is_available() else -1,
    torch_dtype=torch.bfloat16,
    trust_remote_code=True
)


class Conversation:
    def __init__(self, system_prompt=None):
        self.session_id = str(uuid4())
        self.messages = []
        self.last_accessed = datetime.now()

        if system_prompt:
            self.messages.append({
                "role": "system",
                "content": system_prompt
            })

    def add_message(self, role, content):
        self.messages.append({"role": role, "content": content})
        # Keep conversation within token limits
        if len(self.messages) > MAX_HISTORY * 2 + 1:  # System + 10 exchanges
            self.messages = [self.messages[0]] + self.messages[-MAX_HISTORY * 2:]


@app.route('/start', methods=['POST'])
def start_conversation():
    system_prompt = request.json.get('system_prompt')
    with conversation_lock:
        conv = Conversation(system_prompt)
        conversations[conv.session_id] = conv
    return jsonify({"session_id": conv.session_id})


@app.route('/chat', methods=['POST'])
def chat():
    session_id = request.json.get('session_id')
    user_input = request.json.get('message')

    with conversation_lock:
        conv = conversations.get(session_id)
        if not conv:
            return jsonify({"error": "Invalid session ID"}), 400

        # Add user message to history
        conv.add_message("user", user_input)

        # Generate response
        response = pipe(
            conv.messages,
            max_new_tokens=200,
            temperature=0.7,
            return_full_text=False
        )[0]['generated_text']

        # Add assistant response to history
        conv.add_message("assistant", response)
        conv.last_accessed = datetime.now()

    return jsonify({
        "response": response,
        "session_id": session_id
    })


@app.route('/end', methods=['POST'])
def end_conversation():
    session_id = request.json.get('session_id')
    with conversation_lock:
        if session_id in conversations:
            del conversations[session_id]
    return jsonify({"status": "session ended"})


def cleanup_thread():
    """Remove inactive sessions periodically"""
    while True:
        time.sleep(300)  # Run every 5 minutes
        with conversation_lock:
            now = datetime.now()
            expired = [sid for sid, conv in conversations.items()
                       if (now - conv.last_accessed).total_seconds() > SESSION_TIMEOUT]
            for sid in expired:
                del conversations[sid]


if __name__ == '__main__':
    import threading

    threading.Thread(target=cleanup_thread, daemon=True).start()
    app.run(host='0.0.0.0', port=5000, threaded=True)