namespace ForestNET.Lib
{
    /// <summary>
    /// Collection of classes to realize a finite state automation system or state machine.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// Interface declaration to define the shell of a state method which must be used to fill the state machine
        /// </summary>
        /// <param name="p_a_param">optional dynamic list of objects as parameter</param>
        /// <returns>Return code after state method was executed</returns>
        public delegate string StateMethodShell(List<object> p_a_param);

        /* Constant */

        public const string EXIT = "EXIT";

        /* Fields */

        private bool b_ready;
        private readonly List<string> a_states;
        private readonly List<string> a_returnCodes;
        private readonly List<Transition> a_transitions;
        private readonly List<StateMethodContainer> a_stateMethods;

        /* Properties */

        /* Methods */

        /// <summary>
        /// Creates a state machine object which is not ready for executing - declaring which states and return codes are valid
        /// </summary>
        /// <param name="p_a_states">States of state machine; only capital characters; 'EXIT' is not allowed and will be added automatically at the end of the constructor</param>
        /// <param name="p_a_returnCodes">Return codes of state machine; only capital characters; 'EXIT' is not allowed and will be added automatically at the end of the constructor</param>
        /// <exception cref="ArgumentException">States or Return codes parameter are empty</exception>
        public StateMachine(List<string> p_a_states, List<string> p_a_returnCodes)
        {
            if ((p_a_states == null) || (p_a_states.Count < 1))
            {
                throw new ArgumentException("States parameter is empty");
            }

            if ((p_a_returnCodes == null) || (p_a_returnCodes.Count < 1))
            {
                throw new ArgumentException("Return codes parameter is empty");
            }

            this.b_ready = false;
            this.a_states = [];
            this.a_returnCodes = [];
            this.a_transitions = [];
            this.a_stateMethods = [];

            /* filter out 'EXIT' state */
            foreach (string s_state in p_a_states)
            {
                if (s_state != StateMachine.EXIT)
                {
                    this.a_states.Add(s_state.ToUpper());
                }
            }

            /* filter out 'EXIT' return code */
            foreach (string s_returnCode in p_a_returnCodes)
            {
                if (s_returnCode != StateMachine.EXIT)
                {
                    this.a_returnCodes.Add(s_returnCode.ToUpper());
                }
            }

            /* add 'EXIT' state and return code at the end */
            this.a_states.Add(StateMachine.EXIT);
            this.a_returnCodes.Add(StateMachine.EXIT);
        }

        /// <summary>
        /// Add a transition to state machine configuration, with at least one transition state machine is ready for execution
        /// </summary>
        /// <param name="p_s_fromState">Entering state which leads to this transition; only capital characters</param>
        /// <param name="p_s_returnCode">Return code at the end of the current state; only capital characters</param>
        /// <param name="p_s_toState">Next state after combination of entering state and return code; only capital characters</param>
        /// <exception cref="ArgumentException">Invalid state or return code parameter</exception>
        public void AddTransition(string p_s_fromState, string p_s_returnCode, string p_s_toState)
        {
            p_s_fromState = p_s_fromState.ToUpper();
            p_s_returnCode = p_s_returnCode.ToUpper();
            p_s_toState = p_s_toState.ToUpper();

            if (!this.a_states.Contains(p_s_fromState))
            {
                throw new ArgumentException("Invalid state '" + p_s_fromState + "', valid states are [" + ForestNET.Lib.Helper.JoinList(this.a_states, ',') + "]");
            }

            if (!this.a_returnCodes.Contains(p_s_returnCode))
            {
                throw new ArgumentException("Invalid return code '" + p_s_returnCode + "', valid states are [" + ForestNET.Lib.Helper.JoinList(this.a_returnCodes, ',') + "]");
            }

            if (!this.a_states.Contains(p_s_toState))
            {
                throw new ArgumentException("Invalid state '" + p_s_toState + "', valid states are [" + ForestNET.Lib.Helper.JoinList(this.a_states, ',') + "]");
            }

            this.a_transitions.Add(new Transition(p_s_fromState, p_s_returnCode, p_s_toState));

            this.b_ready = true;
        }

        /// <summary>
        /// Execute the current state method depending of state parameter.
        /// optional parameters possible.
        /// </summary>
        /// <param name="p_s_state">information about the current state of the state machine; only capital characters</param>
        /// <param name="p_a_param">optional parameters as dynamic object list</param>
        /// <returns>return code of a state transition</returns>
        /// <exception cref="Exception">any exception from execution of state method</exception>
        /// <exception cref="InvalidOperationException">State machine is not ready, transition configuration must be loaded first</exception>
        public string ExecuteStateMethod(string? p_s_state, List<object>? p_a_param)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("State machine is not ready. Please load a transition configuration first");
            }

            if (p_a_param == null)
            {
                throw new InvalidOperationException("Cannot execute state method with generic list as 'null'");
            }

            /* look for state method in dynamic list */
            foreach (StateMethodContainer o_stateMethodContainer in this.a_stateMethods)
            {
                /* state method state must be equal to state parameter */
                if (o_stateMethodContainer.State.Equals(p_s_state?.ToUpper()))
                {
                    /* execute state method shell with optional parameters */
                    return o_stateMethodContainer.StateMethodShell(p_a_param);
                }
            }

            /* return exit state if no state method was found */
            return StateMachine.EXIT;
        }

        /// <summary>
        /// Adding a state method to dynamic list of state machine
        /// </summary>
        /// <param name="o_stateMethodContainer">state method container</param>
        /// <exception cref="InvalidOperationException">State machine is not ready, transition configuration must be loaded first</exception>
        public void AddStateMethod(StateMethodContainer o_stateMethodContainer)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("State machine is not ready. Please load a transition configuration first");
            }

            bool b_found = false;

            /* look if state of state method is in state dynamic list */
            foreach (string s_state in this.a_states)
            {
                if (o_stateMethodContainer.State.Equals(s_state))
                {
                    b_found = true;
                }
            }

            /* invalid state found */
            if (!b_found)
            {
                throw new InvalidOperationException("Invalid state '" + o_stateMethodContainer.State + "', valid states are [" + ForestNET.Lib.Helper.JoinList(this.a_states, ',') + "]");
            }

            /* add method container to dynamic list */
            this.a_stateMethods.Add(o_stateMethodContainer);
        }

        /// <summary>
        /// Lookup transitions to get the next state of state machine based on current state and return code of it's state method
        /// </summary>
        /// <param name="p_s_currentState">current state of state machine</param>
        /// <param name="p_s_returnCode">return code of state method</param>
        /// <returns>State</returns>
        /// <exception cref="InvalidOperationException">State machine is not ready, transition configuration must be loaded first - or transition configuration is incomplete</exception>
        public string LookupTransitions(string? p_s_currentState, string? p_s_returnCode)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("State machine is not ready. Please load a transition configuration first");
            }

            if (p_s_currentState == null)
            {
                throw new InvalidOperationException("There was no transition found with current state[null].");
            }

            if (p_s_returnCode == null)
            {
                throw new InvalidOperationException("There was no transition found with return code[null].");
            }

            string? s_state = null;

            /* iterate each transition of state machine */
            foreach (Transition o_transition in this.a_transitions)
            {
                bool currentStateMatches = o_transition.FromState.Equals(p_s_currentState.ToUpper());
                bool conditionsMatch = o_transition.ReturnCode.Equals(p_s_returnCode.ToUpper());

                /* state and return code conditions are matching */
                if (currentStateMatches && conditionsMatch)
                {
                    s_state = o_transition.ToState;
                    break;
                }
            }

            /* if state is null our transition configuration is incomplete */
            if (s_state == null)
            {
                throw new InvalidOperationException("There was no transition found for current state[" + p_s_currentState.ToUpper() + "] and return code[" + p_s_returnCode.ToUpper() + "].");
            }

            /* return next state */
            return s_state;
        }

        /* Internal Classes */

        /// <summary>
        /// State method container class to encapsulate a state method as a container; can only be set on creation
        /// </summary>
        public class StateMethodContainer
        {

            /* Fields */

            /* Properties */

            public string State { get; private set; }
            public StateMethodShell StateMethodShell { get; private set; }

            /* Methods */

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="p_s_state">State of state method as string value</param>
            /// <param name="p_o_stateMethodShell">Method shell of state method</param>
            public StateMethodContainer(string p_s_state, StateMethodShell p_o_stateMethodShell)
            {
                this.State = p_s_state.ToUpper();
                this.StateMethodShell = p_o_stateMethodShell;
            }
        }

        /// <summary>
        /// Internal state machine class to encapsulate a transition; can only be set on creation
        /// </summary>
        private class Transition
        {

            /* Fields */

            /* Properties */

            public string FromState { get; private set; }
            public string ReturnCode { get; private set; }
            public string ToState { get; private set; }

            /* Methods */

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="p_s_fromState">Entering state which leads to this transition</param>
            /// <param name="p_s_returnCode">Return code at the end of the current state</param>
            /// <param name="p_s_toState">Next state after combination of entering state and return code</param>
            public Transition(string p_s_fromState, string p_s_returnCode, string p_s_toState)
            {
                this.FromState = p_s_fromState;
                this.ReturnCode = p_s_returnCode;
                this.ToState = p_s_toState;
            }
        }
    }
}
